using System;

namespace PoissonSoft.BinanceApi.UserDataStreams
{
    /// <inheritdoc />
    public abstract class UserDataStream : IUserDataStream
    {


        /// <summary>
        /// Создание экземпляра
        /// </summary>
        /// <param name="apiClient"></param>
        protected UserDataStream(BinanceApiClient apiClient)
        {
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
