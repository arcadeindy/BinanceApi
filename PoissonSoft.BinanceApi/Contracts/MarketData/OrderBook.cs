using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.MarketData
{
    /// <summary>
    /// Содержимое стакана
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// Last Update Id
        /// </summary>
        [JsonProperty("lastUpdateId")]
        public long LastUpdateId { get; set; }

        /// <summary>
        /// Bids
        /// </summary>
        [JsonProperty("bids")]
        public string[][] Bids { get; set; }

        /// <summary>
        /// asks
        /// </summary>
        [JsonProperty("asks")]
        public string[][] Asks { get; set; }
    }
}
