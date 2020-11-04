using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
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
        private readonly BinanceApiClientCredentials credentials;
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
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

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
            // TODO:
            throw new NotImplementedException();
        }

        public void UnsubscribeAll()
        {
            // TODO:
            throw new NotImplementedException();
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
            // TODO:
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
            RestoreSubscriptions();

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

            // TODO: !!!

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
