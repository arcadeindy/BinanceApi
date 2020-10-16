using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts
{
    /// <summary>
    /// Баланс конкретной монеты
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// "asset": "BTC",
        /// </summary>
        [JsonProperty("asset")]
        public string Asset {get; set; }

        /// <summary>
        /// "free": "4723846.89208129",
        /// </summary>
        [JsonProperty("free")]
        public decimal Free { get; set; }

        /// <summary>
        /// "locked": "0.00000000"
        /// </summary>
        [JsonProperty("locked")]
        public decimal Locked { get; set; }
    }
}
