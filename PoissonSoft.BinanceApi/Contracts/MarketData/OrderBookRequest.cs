using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.MarketData
{
    /// <summary>
    /// Запрос на получение биржевого стакана
    /// </summary>
    public class OrderBookRequest
    {
        /// <summary>
        /// Торговый инструмент
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Лимит 5, 10, 20, 50, 100, 500, 1000, 5000
        /// </summary>
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }
    }
}
