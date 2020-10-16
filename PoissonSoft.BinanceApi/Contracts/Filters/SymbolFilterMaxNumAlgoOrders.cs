using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_NUM_ALGO_ORDERS filter defines the maximum number of "algo" orders an account is allowed to have open on a symbol.
    /// "Algo" orders are STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
    /// </summary>
    public class SymbolFilterMaxNumAlgoOrders : SymbolFilter
    {
        /// <summary>
        /// Maximum number of "algo" orders an account is allowed to have open on a symbol
        /// </summary>
        [JsonProperty("maxNumAlgoOrders")]
        public int MaxNumAlgoOrders { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterMaxNumAlgoOrders
            {
                FilterType = FilterType,
                MaxNumAlgoOrders = MaxNumAlgoOrders,
            };
        }

    }
}
