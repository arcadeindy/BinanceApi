using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoissonSoft.BinanceApi.Contracts.MarketDataStream;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Ws;
using PoissonSoft.BinanceApi.Utils;
using Timer = System.Timers.Timer;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    internal class MarketStreamsManager: IMarketStreamsManager, IDisposable
    {
        private const string WS_ENDPOINT = "wss://stream.binance.com:9443/stream";

        private readonly BinanceApiClient apiClient;
        private readonly WebSocketStreamListener streamListener;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<long, SubscriptionWrap>> subscriptions =
            new ConcurrentDictionary<string, ConcurrentDictionary<long, SubscriptionWrap>>();

        private readonly string userFriendlyName = nameof(MarketStreamsManager);
        private TimeSpan reconnectTimeout = TimeSpan.Zero;
        private readonly Timer pongTimer;

        public DataStreamStatus WsConnectionStatus { get; private set; } = DataStreamStatus.Closed;


        public MarketStreamsManager(BinanceApiClient apiClient, BinanceApiClientCredentials credentials)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

            streamListener = new WebSocketStreamListener(apiClient, credentials);
            streamListener.OnConnected += OnConnectToWs;
            streamListener.OnConnectionClosed += OnDisconnect;
            streamListener.OnMessage += OnStreamMessage;

            // The websocket server will send a ping frame every 3 minutes. If the websocket server does not receive a pong
            // frame back from the connection within a 10 minute period, the connection will be disconnected.
            // Unsolicited pong frames are allowed.
            pongTimer = new Timer
            {
                AutoReset = true, 
                Enabled = false,
                Interval = TimeSpan.FromMinutes(3).TotalMilliseconds
            };
            pongTimer.Elapsed += OnPongTimer;
        }


        #region [Interface]

        public SubscriptionInfo SubscribePartialBookDepth(string symbol, int levelsCount, int updateSpeedMs, Action<PartialBookDepthPayload> callbackAction)
        {
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentNullException(nameof(symbol));
            if (callbackAction == null) throw new ArgumentNullException(nameof(callbackAction));

            if (apiClient.MarketDataApi.GetExchangeInfo()?.Symbols.FirstOrDefault(x => x.Symbol == symbol) == null)
            {
                throw new Exception($"Unknown symbol '{symbol}'");
            }

            if (levelsCount <= 7) levelsCount = 5;
            else if (levelsCount <= 15) levelsCount = 10;
            else levelsCount = 20;

            updateSpeedMs = updateSpeedMs <= 550 ? 100 : 1000;

            var subscriptionInfo = new SubscriptionInfo
            {
                Id = GenerateUniqueId(),
                StreamType = DataStreamType.PartialBookDepthStreams,
                BinanceStreamName = $"{symbol.ToLowerInvariant()}@depth{levelsCount}{(updateSpeedMs == 1000 ? string.Empty : "100ms")}",
                Symbol = symbol,
                Parameters = new Dictionary<string, string>
                {
                    ["symbol"] = symbol,
                    ["levels"] = levelsCount.ToString(),
                    ["updateSpeed"] = $"{updateSpeedMs}ms"
                }
            };

            var subscriptionWrap = new SubscriptionWrap
            {
                Info = subscriptionInfo,
                CallbackAction = callbackAction
            };

            var needSubscribeToStream = AddSubscription(subscriptionWrap);

            if (needSubscribeToStream)
            {
                var resp = SubscribeToStream(subscriptionInfo.BinanceStreamName);
                if (!resp.Success)
                    throw new Exception($"{userFriendlyName}. Stream subscription error: {resp.ErrorDescription}");
            }

            return subscriptionInfo;
        }

        public bool Unsubscribe(long subscriptionId)
        {
            var streams = subscriptions.Keys.ToArray();
            foreach (var streamKey in streams)
            {
                if (!subscriptions.TryGetValue(streamKey, out var subscriptionsByKey)
                    || !subscriptionsByKey.ContainsKey(subscriptionId)) continue;

                if (!subscriptionsByKey.TryRemove(subscriptionId, out _)) return true;

                if (!subscriptionsByKey.IsEmpty) return true;

                if (!subscriptions.TryRemove(streamKey, out _)) return true;

                var resp = UnsubscribeStream(streamKey);
                if (!resp.Success)
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Stream unsubscription error: {resp.ErrorDescription}");
                }

                return resp.Success;

            }

            apiClient.Logger.Error($"{userFriendlyName}. Не удалось отписаться от подписки {subscriptionId}");
            return false;
        }

        public void UnsubscribeAll()
        {
            if (WsConnectionStatus == DataStreamStatus.Active)
            {
                var resp = GetAllStreams();
                if (!resp.Success) 
                    throw new Exception($"{userFriendlyName}. Не удалось получить список активных подписок");

                if (resp.Data?.Any() == true)
                {
                    foreach (var streamKey in resp.Data)
                    {
                        UnsubscribeStream(streamKey);
                    }
                }
            }

            subscriptions.Clear();
        }

        #endregion

        #region [Subscription management]

        private long lastId;
        
        private class SubscriptionWrap
        {
            public SubscriptionInfo Info { get; set; }

            public object CallbackAction { get; set; }
        }

        private long GenerateUniqueId()
        {
            return Interlocked.Increment(ref lastId);
        }

        private bool AddSubscription(SubscriptionWrap sw)
        {
            var needSubscribeToChannel = false;
            if (!subscriptions.TryGetValue(sw.Info.BinanceStreamName, out var streamSubscriptions))
            {
                streamSubscriptions = new ConcurrentDictionary<long, SubscriptionWrap>();
                if (subscriptions.TryAdd(sw.Info.BinanceStreamName, streamSubscriptions))
                {
                    needSubscribeToChannel = true;
                }
                else
                {
                    subscriptions.TryGetValue(sw.Info.BinanceStreamName, out streamSubscriptions);
                }
            }

            if (streamSubscriptions == null)
                throw new Exception($"{userFriendlyName}. Unexpected error. Can not add key '{sw.Info.BinanceStreamName}' " +
                                    $"to {nameof(subscriptions)} dictionary");

            streamSubscriptions[sw.Info.Id] = sw;

            return needSubscribeToChannel;
        }

        private void RestoreSubscriptions()
        {
            var currentStreams = subscriptions.Keys.ToArray();
            if (!currentStreams.Any()) return;

            foreach (var streamKey in currentStreams)
            {
                var resp = SubscribeToStream(streamKey);
                if (!resp.Success)
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Не удалось возобновить подписку на stream '{streamKey}'. " +
                                           $"Ошибка: {resp.ErrorDescription}");
                }
            }
        }

        private CommandResponse<object> SubscribeToStream(string binanceStreamName)
        {
            while (!disposed && WsConnectionStatus != DataStreamStatus.Active)
            {
                Open();
                if (WsConnectionStatus != DataStreamStatus.Active)
                    Thread.Sleep(500);
            }

            var request = new CommandRequest
            {
                Method = CommandRequestMethod.Subscribe,
                Parameters = new object[] {binanceStreamName},
                RequestId = GenerateUniqueId()
            };

            return ProcessRequest<object>(request);
        }

        private CommandResponse<object> UnsubscribeStream(string binanceStreamName)
        {
            if (WsConnectionStatus != DataStreamStatus.Active) return new CommandResponse<object>
            {
                Success = true
            };

            var request = new CommandRequest
            {
                Method = CommandRequestMethod.Unsubscribe,
                Parameters = new object[] { binanceStreamName },
                RequestId = GenerateUniqueId()
            };

            return ProcessRequest<object>(request);
        }

        private CommandResponse<string[]> GetAllStreams()
        {
            if (WsConnectionStatus != DataStreamStatus.Active) return new CommandResponse<string[]>
            {
                Success = true,
                Data = Array.Empty<string>()
            };

            var request = new CommandRequest
            {
                Method = CommandRequestMethod.ListSubscriptions,
                RequestId = GenerateUniqueId()
            };

            return ProcessRequest<string[]>(request);
        }

        #endregion

        #region [Request processing]

        private class CommandResponse<T>
        {
            public bool Success { get; set; }

            public T Data { get; set; }

            public string ErrorDescription { get; set; }
        }

        private class ManualResetEventPool : ObjectPool<ManualResetEventSlim>
        {
            public ManualResetEventPool()
                : base(startSize: 20, minSize: 10, sizeIncrement: 5, availableLimit: 200)
            { }

            // Новый экземпляр объекта создаётся в не сигнальном состоянии
            protected override ManualResetEventSlim CreateEntity()
            {
                return new ManualResetEventSlim();
            }
        }

        private class ResponseWaiter
        {
            public CommandRequest Request { get; }

            public ManualResetEventSlim SyncEvent { get; }

            public CommandResponse<object> Response { get; set; }

            public ResponseWaiter(CommandRequest request, ManualResetEventSlim syncEvent)
            {
                Request = request;
                SyncEvent = syncEvent;
            }
        }

        private readonly ManualResetEventPool manualResetEventPool = new ManualResetEventPool();

        /// <summary>
        /// Время ожидания (в миллисекундах) обработки отправленного запроса
        /// </summary>
        private const int WAIT_FOR_REQUEST_PROCESSING_MS = 10_000;

        private readonly ConcurrentDictionary<long, ResponseWaiter> responseWaiters =
            new ConcurrentDictionary<long, ResponseWaiter>();

        private CommandResponse<T> ProcessRequest<T>(CommandRequest request)
        {
            bool requestWasProcessed;
            ManualResetEventSlim syncEvent = null;
            ResponseWaiter waiter = null;
            try
            {
                if (!manualResetEventPool.TryGetEntity(out syncEvent))
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Couldn't get the synchronization object from the pool");
                }
                syncEvent?.Reset();
                waiter = new ResponseWaiter(request, syncEvent);
                responseWaiters.TryAdd(waiter.Request.RequestId, waiter);
                streamListener.SendMessage(JsonConvert.SerializeObject(request));

                requestWasProcessed = syncEvent != null && syncEvent.Wait(WAIT_FOR_REQUEST_PROCESSING_MS);
            }
            finally
            {
                if (syncEvent != null) manualResetEventPool.ReturnToPool(syncEvent);
                if (waiter?.Request != null) responseWaiters.TryRemove(waiter.Request.RequestId, out _);
            }

            if (!requestWasProcessed)
            {
                return new CommandResponse<T>
                {
                    Success = false,
                    ErrorDescription = "A response to the request has not been received within the " +
                                       $"specified timeout ({WAIT_FOR_REQUEST_PROCESSING_MS} мс)"
                };
            }

            if (waiter.Response?.Success == null)
            {
                return new CommandResponse<T>
                {
                    Success = false,
                    ErrorDescription = "Incorrect response: " +
                                       $"{(waiter.Response == null ? "NULL" : JsonConvert.SerializeObject(waiter.Response))}"
                };
            }
            
            if (waiter.Response?.Success == false)
            {
                return new CommandResponse<T>
                {
                    Success = false,
                    ErrorDescription = waiter.Response.ErrorDescription ?? "Error"
                };
            }

            T data;
            try
            {
                data = (T) waiter.Response.Data;
            }
            catch
            {
                return new CommandResponse<T>
                {
                    Success = false,
                    ErrorDescription = "Incorrect data in Response: " +
                                       $"{(waiter.Response.Data == null ? "NULL" : waiter.Response.Data.GetType().Name)}"
                };
            }

            return new CommandResponse<T>
            {
                Success = true,
                ErrorDescription = waiter.Response.ErrorDescription,
                Data = data
            };

        }


        #endregion

        #region [Connection management]

        private void Open()
        {
            if (WsConnectionStatus != DataStreamStatus.Closed) return;
            WsConnectionStatus = DataStreamStatus.Connecting;
            TryConnectToWebSocket();
        }

        public void Close()
        {
            if (WsConnectionStatus == DataStreamStatus.Closed) return;

            UnsubscribeAll();

            WsConnectionStatus = DataStreamStatus.Closing;
            try
            {
                streamListener?.Close();
            }
            catch (Exception e)
            {
                apiClient.Logger.Error($"{userFriendlyName}. Exception when closing Websocket connection:\n{e}");
            }

            apiClient.Logger.Info($"{userFriendlyName}. Websocket connection closed");
            WsConnectionStatus = DataStreamStatus.Closed;
        }

        private void OnConnectToWs(object sender, EventArgs e)
        {
            WsConnectionStatus = DataStreamStatus.Active;
            reconnectTimeout = TimeSpan.Zero;
            apiClient.Logger.Info($"{userFriendlyName}. Successfully connected to stream!");
            pongTimer.Enabled = true;
        }

        private void OnDisconnect(object sender, (WebSocketCloseStatus? CloseStatus, string CloseStatusDescription) e)
        {
            pongTimer.Enabled = false;

            if (disposed || WsConnectionStatus == DataStreamStatus.Closing) return;

            WsConnectionStatus = DataStreamStatus.Reconnecting;
            if (reconnectTimeout.TotalSeconds < 15) reconnectTimeout += TimeSpan.FromSeconds(1);
            apiClient.Logger.Error($"{userFriendlyName}. WebSocket was disconnected. Try reconnect again after {reconnectTimeout}.");
            Task.Run(() =>
            {
                Task.Delay(reconnectTimeout);
                TryConnectToWebSocket();
                RestoreSubscriptions();
            });
        }

        private void TryConnectToWebSocket()
        {
            while (true)
            {
                if (disposed) return;
                try
                {
                    streamListener.Connect(WS_ENDPOINT);
                    return;
                }
                catch (Exception e)
                {
                    if (reconnectTimeout.TotalSeconds < 15) reconnectTimeout += TimeSpan.FromSeconds(1);
                    apiClient.Logger.Error($"{userFriendlyName}. WebSocket connection failed. Try again after {reconnectTimeout}. Exception:\n{e}");
                    Thread.Sleep(reconnectTimeout);
                }
            }
        }

        // The websocket server will send a ping frame every 3 minutes. If the websocket server does not receive a pong
        // frame back from the connection within a 10 minute period, the connection will be disconnected.
        // Unsolicited pong frames are allowed.
        private void OnPongTimer(object sender, ElapsedEventArgs e)
        {
            streamListener.SendMessage("pong");
        }

        #endregion

        #region [Payload processing]

        private void OnStreamMessage(object sender, string message)
        {
            Task.Run(() =>
            {
                try
                {
                    ProcessStreamMessage(message);
                }
                catch (Exception e)
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Exception when processing payload.\n" +
                                 $"Message: {message}\n" +
                                 $"Exception: {e}");
                }
            });
        }

        private void ProcessStreamMessage(string message)
        {
            if (apiClient.IsDebug)
            {
                apiClient.Logger.Debug($"{userFriendlyName}. New payload received:\n{message}");
            }

            if (message.ToUpperInvariant() == "PING")
            {
                // PING message
                return;
            }

            var jToken = JToken.Parse(message);

            if (jToken.Type != JTokenType.Object)
            {
                apiClient.Logger.Error($"{userFriendlyName}. An unexpected message was received from the server.\n" +
                                       $"Type: {jToken.Type}, Message: {message}");
                return;
            }
            var jObject = (JObject)jToken;

            // Снимок с данными
            if (jObject.ContainsKey("stream"))
            {
                if (!jObject.ContainsKey("data"))
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Stream message does not contains 'data' field.\n" +
                                           $"Message: {message}");
                    return;
                }
                ProcessPayload(jObject["stream"]?.ToString(), jObject["data"]);
                return;
            }

            // Ответ на запрос
            if (jObject.ContainsKey("result") && jObject.ContainsKey("id"))
            {
                ProcessResponse(jObject);
                return;
            }

            // Ошибка
            var waitingRequests = responseWaiters.Values.ToArray().Select(x => JsonConvert.SerializeObject(x.Request));

            apiClient.Logger.Error($"{userFriendlyName}. Получена информация об ошибке: {message}\n" +
                                   $"Отправленные запросы, ожидающие ответа:\n{string.Join("\n", waitingRequests)}");

        }

        private void ProcessPayload(string streamName, JToken streamData)
        {
            if (!subscriptions.TryGetValue(streamName, out var activeSubscriptionsDic))
            {
                apiClient.Logger.Error($"{userFriendlyName}. Получено сообщение из потока '{streamName}', " +
                                       "однако подписок на данный поток не обнаружено");
                return;
            }

            var activeSubscriptions = activeSubscriptionsDic.Values.ToList();
            if (!activeSubscriptions.Any()) return;

            var payloadType = activeSubscriptions.First().Info.StreamType;
            switch (payloadType)
            {
                case DataStreamType.AggregateTrade:
                    // Not implemented yet
                    break;
                case DataStreamType.Trade:
                    // Not implemented yet
                    break;
                case DataStreamType.Candlestick:
                    // Not implemented yet
                    break;
                case DataStreamType.IndividualSymbolMiniTicker:
                    // Not implemented yet
                    break;
                case DataStreamType.AllMarketMiniTickers:
                    // Not implemented yet
                    break;
                case DataStreamType.IndividualSymbolTicker:
                    // Not implemented yet
                    break;
                case DataStreamType.AllMarketTickers:
                    // Not implemented yet
                    break;
                case DataStreamType.IndividualSymbolBookTicker:
                    // Not implemented yet
                    break;
                case DataStreamType.AllBookTickers:
                    // Not implemented yet
                    break;

                case DataStreamType.PartialBookDepthStreams:
                    activeSubscriptions.ForEach(sw =>
                    {
                        if (sw.CallbackAction is Action<PartialBookDepthPayload> callback)
                        {
                            callback(streamData?.ToObject<PartialBookDepthPayload>());
                        }
                    });
                    break;

                case DataStreamType.DiffDepth:
                    // Not implemented yet
                    break;
                default:
                    apiClient.Logger.Error($"{userFriendlyName}. Unknown Payload type '{payloadType}'");
                    break;
            }
        }

        private void ProcessResponse(JObject response)
        {
            long requestId;
            try
            {
                requestId = response["id"]?.ToObject<long>() 
                            ?? throw new Exception("При конвертации поля id в long получили значение NULL");
            }
            catch (Exception ex)
            {
                apiClient.Logger.Error($"{userFriendlyName}. При обработке ответа на запрос не удалось получить ID запроса. " +
                                       $"Message: {response}\nException {ex}");
                return;
            }

            if (!responseWaiters.TryGetValue(requestId, out var waiter))
            {
                apiClient.Logger.Error($"{userFriendlyName}. Среди ожидающих ответа запросов не удалось найти запрос " +
                                       $"с ID={requestId}");
                return;
            }

            var cmdResp = new CommandResponse<object>
            {
                Success = true
            };
            try
            {
                switch (waiter.Request.Method)
                {
                    case CommandRequestMethod.Subscribe:
                    case CommandRequestMethod.Unsubscribe:
                        cmdResp.Data = null;
                        break;
                    case CommandRequestMethod.ListSubscriptions:
                        cmdResp.Data = response["result"]?.ToObject<string[]>();
                        break;
                    case CommandRequestMethod.SetProperty:
                        cmdResp.Data = null;
                        break;
                    case CommandRequestMethod.GetProperty:
                        cmdResp.Data = response["result"]?.ToObject<bool>();
                        break;
                    default:
                        cmdResp.Success = false;
                        cmdResp.ErrorDescription = $"Unknown Request Method '{waiter.Request.Method}'";
                        break;
                }
            }
            catch (Exception e)
            {
                cmdResp.Success = false;
                cmdResp.ErrorDescription = e.Message;
            }

            waiter.Response = cmdResp;
            waiter.SyncEvent.Set();
        }

        #endregion

        #region [Dispose pattern]

        private bool disposed;
        /// <summary/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (streamListener != null)
                {
                    Close();
                    streamListener.OnConnected -= OnConnectToWs;
                    streamListener.OnConnectionClosed -= OnDisconnect;
                    streamListener.OnMessage -= OnStreamMessage;
                    streamListener?.Dispose();
                }

                manualResetEventPool?.Dispose();

                if (pongTimer != null)
                {
                    pongTimer.Enabled = false;
                    pongTimer.Elapsed -= OnPongTimer;
                    pongTimer.Dispose();
                }
            }

            disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
        
    }
}
