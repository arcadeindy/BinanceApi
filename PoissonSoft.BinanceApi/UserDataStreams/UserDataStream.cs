using System;
using NLog;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.Transport.Rest;

namespace PoissonSoft.BinanceApi.UserDataStreams
{
    /// <inheritdoc />
    public abstract class UserDataStream : IUserDataStream
    {
        private readonly ILogger logger;
        private string listenKey;

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        protected UserDataStream(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Status = UserDataStreamStatus.Closed;
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
        public void Open()
        {
            try
            {
                listenKey = CreateListenKey();
            }
            catch (Exception e)
            {
                logger.Error($"Can not create Listen Key. Exception:\n{e}");
            }

            // TODO: 
        }

        /// <inheritdoc />
        public void Close()
        {
            // TODO:

            try
            {
                CloseListenKey(listenKey);
            }
            catch (Exception e)
            {
                logger.Error($"Exception when closing Listen Key:\n{e}");
            }
        }

        /// <summary>
        /// Start a new user data stream. The stream will close after 60 minutes unless a keepalive is sent.
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

    }
}
