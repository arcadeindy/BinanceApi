using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The PRICE_FILTER defines the price rules for a symbol.
    /// </summary>
    public class SymbolFilterPriceFilter: SymbolFilter
    {
        /// <summary>
        /// minPrice defines the minimum price/stopPrice allowed; disabled on minPrice == 0.
        /// </summary>
        [JsonProperty("minPrice")]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// maxPrice defines the maximum price/stopPrice allowed; disabled on maxPrice == 0.
        /// </summary>
        [JsonProperty("maxPrice")]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// tickSize defines the intervals that a price/stopPrice can be increased/decreased by; disabled on tickSize == 0.
        /// </summary>
        [JsonProperty("tickSize")]
        public decimal TickSize { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterPriceFilter
            {
                FilterType = FilterType,
                MinPrice = MinPrice,
                MaxPrice = MaxPrice,
                TickSize = TickSize,
            };
        }
    }
}
