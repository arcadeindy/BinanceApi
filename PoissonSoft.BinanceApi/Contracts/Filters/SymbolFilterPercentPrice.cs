using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The PERCENT_PRICE filter defines valid range for a price based on the average of the previous trades.
    /// </summary>
    public class SymbolFilterPercentPrice: SymbolFilter
    {

        /// <summary>
        /// price &lt;= weightedAveragePrice * multiplierUp
        /// </summary>
        [JsonProperty("multiplierUp")]
        public decimal MultiplierUp { get; set; }

        /// <summary>
        /// price >= weightedAveragePrice * multiplierDown
        /// </summary>
        [JsonProperty("multiplierDown")]
        public decimal MultiplierDown { get; set; }

        /// <summary>
        /// avgPriceMins is the number of minutes the average price is calculated over. 0 means the last price is used.
        /// </summary>
        [JsonProperty("avgPriceMins")]
        public int AvgPriceMins { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterPercentPrice
            {
                FilterType = FilterType,
                MultiplierUp = MultiplierUp,
                MultiplierDown = MultiplierDown,
                AvgPriceMins = AvgPriceMins,
            };
        }

    }
}
