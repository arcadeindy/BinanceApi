using System;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Баланс конкретной монеты
    /// </summary>
    public class Balance: ICloneable
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

        /// <inheritdoc />
        public object Clone()
        {
            return new Balance
            {
                Asset = Asset,
                Free = Free,
                Locked = Locked,
            };
        }
    }
}
