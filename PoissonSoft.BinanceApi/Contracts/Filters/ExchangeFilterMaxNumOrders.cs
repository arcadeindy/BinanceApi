using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_NUM_ORDERS filter defines the maximum number of orders an account is allowed to have open on the exchange.
    /// Note that both "algo" orders and normal orders are counted for this filter.
    /// </summary>
    public class ExchangeFilterMaxNumOrders : ExchangeFilter
    {
        /// <summary>
        /// Maximum number of orders an account is allowed to have open on the exchange
        /// </summary>
        [JsonProperty("maxNumOrders")]
        public int MaxNumOrders { get; set; }

        /// <inheritdoc />
        protected override ExchangeFilter CreateInstanceForClone()
        {
            return new ExchangeFilterMaxNumOrders
            {
                FilterType = FilterType,
                MaxNumOrders = MaxNumOrders
            };
        }
    }
}
