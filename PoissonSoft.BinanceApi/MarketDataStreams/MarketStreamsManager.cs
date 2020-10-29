using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using PoissonSoft.BinanceApi.Contracts.MarketDataStream;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Ws;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    internal class MarketStreamsManager: IMarketStreamsManager, IDisposable
    {
        private const string WS_ENDPOINT = "wss://stream.binance.com:9443/stream";

        private readonly BinanceApiClient apiClient;
        private readonly BinanceApiClientCredentials credentials;
        private readonly WebSocketStreamListener streamListener;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, SubscriptionWrap>> subscriptions =
            new ConcurrentDictionary<string, ConcurrentDictionary<int, SubscriptionWrap>>();

        private readonly string userFriendlyName = nameof(MarketStreamsManager);
        private TimeSpan reconnectTimeout = TimeSpan.Zero;

        public DataStreamStatus WsConnectionStatus { get; private set; } = DataStreamStatus.Closed;


        public MarketStreamsManager(BinanceApiClient apiClient, BinanceApiClientCredentials credentials)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));

            streamListener = new WebSocketStreamListener(apiClient.Logger, credentials);
            streamListener.OnConnected += OnConnectToWs;
            streamListener.OnConnectionClosed += OnDisconnect;
            streamListener.OnMessage += OnStreamMessage;
        }


        public SubscriptionInfo SubscribePartialBookDepth(string symbol, int levelsCount, int updateSpeedMs, Action<PartialBookDepthPayload> callbackAction)
        {
            // TODO:
            throw new NotImplementedException();
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

        #region [Subscription management]

        private class SubscriptionWrap
        {
            public SubscriptionInfo Info { get; set; }

            public object CallbackAction { get; set; }
        }

        private void RestoreSubscriptions()
        {
            // TODO:
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

            apiClient.Logger.Info($"{userFriendlyName}. Websocket connection closed");
            WsConnectionStatus = DataStreamStatus.Closed;
        }

        private void OnConnectToWs(object sender, EventArgs e)
        {
            WsConnectionStatus = DataStreamStatus.Active;
            reconnectTimeout = TimeSpan.Zero;
            apiClient.Logger.Info($"{userFriendlyName}. Successfully connected to stream!");
            RestoreSubscriptions();
        }

        private void OnDisconnect(object sender, (WebSocketCloseStatus? CloseStatus, string CloseStatusDescription) e)
        {
            if (disposed) return;
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

            // TODO:

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
                    streamListener.OnConnected -= OnConnectToWs;
                    streamListener.OnConnectionClosed -= OnDisconnect;
                    streamListener.OnMessage -= OnStreamMessage;
                    streamListener?.Dispose();
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
