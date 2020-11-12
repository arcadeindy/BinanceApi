using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Deposit Address
    /// </summary>
    public class DepositAddress
    {
        /// <summary>
        /// "address": "1HPn8Rx2y6nNSfagQBKy27GB99Vbzg89wv",
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// "coin": "BTC",
        /// </summary>
        [JsonProperty("coin")]
        public string Coin { get; set; }

        /// <summary>
        /// "tag": "",
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// "url": "https://btc.com/1HPn8Rx2y6nNSfagQBKy27GB99Vbzg89wv"
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
