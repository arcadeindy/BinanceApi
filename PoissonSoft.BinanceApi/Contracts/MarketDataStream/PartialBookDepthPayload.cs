using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.MarketDataStream
{
    /// <summary>
    /// Partial Book Depth Streams payload
    /// </summary>
    public class PartialBookDepthPayload
    {
        /// <summary>
        /// Last update ID
        /// </summary>
        [JsonProperty("lastUpdateId")]
        public long LastUpdateId { get; set; }

        /// <summary>
        /// Bids [Price, Quantity]
        /// </summary>
        [JsonProperty("bids")]
        public decimal[][] Bids { get; set; }

        /// <summary>
        /// Asks [Price, Quantity]
        /// </summary>
        [JsonProperty("asks")]
        public decimal[][] Asks { get; set; }
    }
}
