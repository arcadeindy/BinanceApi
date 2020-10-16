using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MARKET_LOT_SIZE filter defines the quantity (aka "lots" in auction terms) rules for MARKET orders on a symbol.
    /// </summary>
    public class SymbolFilterMarketLotSize : SymbolFilter
    {

        /// <summary>
        /// minQty defines the minimum quantity allowed.
        /// </summary>
        [JsonProperty("minQty")]
        public decimal MinQty { get; set; }

        /// <summary>
        /// maxQty defines the maximum quantity allowed.
        /// </summary>
        [JsonProperty("maxQty")]
        public decimal MaxQty { get; set; }

        /// <summary>
        /// stepSize defines the intervals that a quantity can be increased/decreased by.
        /// </summary>
        [JsonProperty("stepSize")]
        public decimal StepSize { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterMarketLotSize
            {
                FilterType = FilterType,
                MinQty = MinQty,
                MaxQty = MaxQty,
                StepSize = StepSize,
            };
        }

    }
}
