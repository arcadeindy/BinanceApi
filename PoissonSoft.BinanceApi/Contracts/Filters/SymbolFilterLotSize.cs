using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The LOT_SIZE filter defines the quantity (aka "lots" in auction terms) rules for a symbol.
    /// </summary>
    public class SymbolFilterLotSize: SymbolFilter
    {

        /// <summary>
        /// minQty defines the minimum quantity/icebergQty allowed.
        /// </summary>
        [JsonProperty("minQty")]
        public decimal MinQty { get; set; }

        /// <summary>
        /// maxQty defines the maximum quantity/icebergQty allowed.
        /// </summary>
        [JsonProperty("maxQty")]
        public decimal MaxQty { get; set; }

        /// <summary>
        /// stepSize defines the intervals that a quantity/icebergQty can be increased/decreased by.
        /// </summary>
        [JsonProperty("stepSize")]
        public decimal StepSize { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterLotSize
            {
                FilterType = FilterType,
                MinQty = MinQty,
                MaxQty = MaxQty,
                StepSize = StepSize,
            };
        }

    }
}
