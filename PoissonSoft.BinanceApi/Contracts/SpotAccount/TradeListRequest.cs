using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Request object used to get trades
    /// </summary>
    public class TradeListRequest
    {
        /// <summary/>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Start time timestamp in ms (optional)
        /// </summary>
        [JsonProperty("startTime")]
        public long? StartTimeMs { get; set; }

        /// <summary>
        /// End time timestamp in ms (optional)
        /// </summary>
        [JsonProperty("endTime")]
        public long? EndTime { get; set; }

        /// <summary>
        /// TradeId to fetch from. Default gets most recent trades. (optional)
        /// If fromId is set, it will get id >= that fromId. Otherwise most recent trades are returned.
        /// </summary>
        [JsonProperty("fromId")]
        public long? FromId { get; set; }

        /// <summary>
        /// Default 500; max 1000. (optional)
        /// </summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }
    }


}
