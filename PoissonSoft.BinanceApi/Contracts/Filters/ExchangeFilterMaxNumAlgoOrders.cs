using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_ALGO_ORDERS filter defines the maximum number of "algo" orders an account is allowed to have open on the exchange.
    /// "Algo" orders are STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
    /// </summary>
    public class ExchangeFilterMaxNumAlgoOrders : ExchangeFilter
    {
        /// <summary>
        /// Maximum number of "algo" orders an account is allowed to have open on the exchange
        /// </summary>
        [JsonProperty("maxNumAlgoOrders")]
        public int MaxNumAlgoOrders { get; set; }

        /// <inheritdoc />
        protected override ExchangeFilter CreateInstanceForClone()
        {
            return new ExchangeFilterMaxNumAlgoOrders
            {
                FilterType = FilterType,
                MaxNumAlgoOrders = MaxNumAlgoOrders
            };
        }
    }
}
