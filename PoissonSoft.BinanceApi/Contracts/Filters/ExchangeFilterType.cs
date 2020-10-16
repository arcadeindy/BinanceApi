using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{

    /// <summary>
    /// Типы фильтров для всей биржи
    /// https://binance-docs.github.io/apidocs/spot/en/#filters
    /// </summary>
    public enum ExchangeFilterType
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
        /// </summary>
        Unknown,

        /// <summary>
        /// The MAX_NUM_ORDERS filter defines the maximum number of orders an account is allowed to have open on the exchange.
        /// Note that both "algo" orders and normal orders are counted for this filter.
        /// </summary>
        [EnumMember(Value = "EXCHANGE_MAX_NUM_ORDERS")]
        ExchangeMaxNumOrders,

        /// <summary>
        /// The MAX_ALGO_ORDERS filter defines the maximum number of "algo" orders an account is allowed to have open on the exchange.
        /// "Algo" orders are STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
        /// </summary>
        [EnumMember(Value = "EXCHANGE_MAX_NUM_ALGO_ORDERS")]
        ExchangeMaxNumAlgoOrders,
    }
}
