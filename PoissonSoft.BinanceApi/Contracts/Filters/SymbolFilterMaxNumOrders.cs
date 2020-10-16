using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_NUM_ORDERS filter defines the maximum number of orders an account is allowed to have open on a symbol.
    /// Note that both "algo" orders and normal orders are counted for this filter.
    /// </summary>
    public class SymbolFilterMaxNumOrders : SymbolFilter
    {
        /// <summary>
        /// Maximum number of orders an account is allowed to have open on a symbol
        /// </summary>
        [JsonProperty("maxNumOrders")]
        public int MaxNumOrders { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterMaxNumOrders
            {
                FilterType = FilterType,
                MaxNumOrders = MaxNumOrders,
            };
        }
    }
}
