using System;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PoissonSoft.BinanceApi.Contracts.Serialization;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.Transport.Ws;
using Timer = System.Timers.Timer;

namespace PoissonSoft.BinanceApi.UserDataStreams
{
    /// <summary>
    /// User Data Stream
    /// </summary>
    public abstract class UserDataStream : IUserDataStream, IDisposable
    {
        private const string WS_BASE_ENDPOINT = "wss://stream.binance.com:9443";

        private readonly BinanceApiClient apiClient;
        private readonly BinanceApiClientCredentials credentials;
        private readonly string userFriendlyName = nameof(UserDataStream);

        private string listenKey;
        private Timer pingTimer;
        private WebSocketStreamListener streamListener;
        private TimeSpan reconnectTimeout = TimeSpan.Zero;
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        protected UserDataStream(BinanceApiClient apiClient, BinanceApiClientCredentials credentials)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Status = UserDataStreamStatus.Closed;


            serializerSettings = new JsonSerializerSettings
            {
                Context = new StreamingContext(StreamingContextStates.All,
                    new SerializationContext { Logger = apiClient.Logger }),
                ContractResolver = new CaseSensitiveContractResolver(),
            };
        }


        /// <inheritdoc />
        public UserDataStreamType StreamType { get; protected set; }

        /// <inheritdoc />
        public string Symbol { get; protected set; }

        /// <inheritdoc />
        public UserDataStreamStatus Status { get; protected set; }

        /// <inheritdoc />
        public event EventHandler<AccountUpdatePayload> OnAccountUpdate;

        /// <inheritdoc />
        public event EventHandler<BalanceUpdatePayload> OnBalanceUpdate;

        /// <inheritdoc />
        public event EventHandler<OrderExecutionReportPayload> OnOrderExecuteEvent;

        /// <inheritdoc />
        public event EventHandler<OrderListStatusPayload> OnOrderListStatusEvent;

        /// <inheritdoc />

        #region [Connection management]

        public void Open()
        {
            if (Status != UserDataStreamStatus.Closed) return;
            Status = UserDataStreamStatus.Connecting;

            try
            {
                listenKey = CreateListenKey();
            }
            catch (Exception e)
            {
                apiClient.Logger.Error($"{userFriendlyName}. Can not create Listen Key. Exception:\n{e}");
                Status = UserDataStreamStatus.Closed;
                return;
            }

            pingTimer = new Timer(TimeSpan.FromMinutes(30).TotalMilliseconds) {AutoReset = true};
            pingTimer.Elapsed += OnPingTimer;
            pingTimer.Enabled = true;

            streamListener = new WebSocketStreamListener(apiClient.Logger, credentials);
            streamListener.OnConnected += OnConnectToStream;
            streamListener.OnConnectionClosed += OnDisconnect;
            streamListener.OnMessage += OnStreamMessage;
            TryConnectToWebSocket();
        }

        /// <inheritdoc />
        public void Close()
        {
            streamListener.OnConnected -= OnConnectToStream;
            streamListener.OnConnectionClosed -= OnDisconnect;
            streamListener.OnMessage -= OnStreamMessage;
            streamListener.Dispose();
            streamListener = null;

            pingTimer.Enabled = false;
            pingTimer.Elapsed -= OnPingTimer;
            pingTimer.Dispose();
            pingTimer = null;

            try
            {
                CloseListenKey(listenKey);
            }
            catch (Exception e)
            {
                apiClient.Logger.Error($"{userFriendlyName}. Exception when closing Listen Key:\n{e}");
            }

            apiClient.Logger.Info($"{userFriendlyName}. Connection closed");
            Status = UserDataStreamStatus.Closed;
        }

        private void OnPingTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                KeepAliveListenKey(listenKey);
            }
            catch (Exception ex)
            {
                apiClient.Logger.Error($"{userFriendlyName}. Exception when send ping to Listen Key:\n{ex}");
            }
        }

        /// <summary>
        /// Start a new user data stream. The stream will close after 60 minutes unless a keep-alive is sent.
        /// If the account has an active listenKey, that listenKey will be returned and its validity will be extended for 60 minutes.
        /// </summary>
        /// <returns></returns>
        protected abstract string CreateListenKey();

        /// <summary>
        /// Keep alive a user data stream to prevent a time out. User data streams will close after 60 minutes.
        /// It's recommended to send a ping about every 30 minutes.
        /// </summary>
        protected abstract void KeepAliveListenKey(string key);

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        protected abstract void CloseListenKey(string key);

        private void OnConnectToStream(object sender, EventArgs e)
        {
            Status = UserDataStreamStatus.Active;
            reconnectTimeout = TimeSpan.Zero;
            apiClient.Logger.Info($"{userFriendlyName}. Successfully connected!");
        }

        private void OnDisconnect(object sender, (WebSocketCloseStatus? CloseStatus, string CloseStatusDescription) e)
        {
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
                    streamListener.Connect($"{WS_BASE_ENDPOINT}/ws/{listenKey}");
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

            var baseMsg = JsonConvert.DeserializeObject<PayloadBase>(message, serializerSettings);
            switch (baseMsg.EventType)
            {
                // outboundAccountPosition is sent any time an account balance has changed and contains the assets
                // that were possibly changed by the event that generated the balance change.
                case "outboundAccountPosition":
                    OnAccountUpdate?.Invoke(this, JsonConvert.DeserializeObject<AccountUpdatePayload>(message, serializerSettings));
                    break;

                // outboundAccountInfo has been deprecated and will be removed in the future.
                // It is recommended to use outboundAccountPosition instead. 
                case "outboundAccountInfo":
                    // Ignore because this event is deprecated
                    break;

                // Balance Update occurs during the following:
                //   - Deposits or withdrawals from the account
                //   - Transfer of funds between accounts(e.g.Spot to Margin)
                case "balanceUpdate":
                    OnBalanceUpdate?.Invoke(this, JsonConvert.DeserializeObject<BalanceUpdatePayload>(message, serializerSettings));
                    break;

                // Событие, информирующее об изменении ордера
                case "executionReport":
                    OnOrderExecuteEvent?.Invoke(this, JsonConvert.DeserializeObject<OrderExecutionReportPayload>(message, serializerSettings));
                    break;

                // If the order is an OCO, an event will be displayed named ListStatus in addition to the executionReport event.
                case "listStatus":
                    OnOrderListStatusEvent?.Invoke(this, JsonConvert.DeserializeObject<OrderListStatusPayload>(message, serializerSettings));
                    break;

                default:
                    apiClient.Logger.Error($"{userFriendlyName}. Unknown payload Event Type '{baseMsg.EventType}'\n" +
                                 $"Message: {message}");
                    break;

            }
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
                pingTimer?.Dispose();
                streamListener?.Dispose();
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
