using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Filters;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.MarketDataStream
{

    /// <summary>
    /// Request to manage Market Data Streams
    /// </summary>
    public class CommandRequest
    {
        /// <summary>
        /// Method
        /// </summary>
        [JsonProperty("method")]
        [JsonConverter(typeof(StringEnumExConverter), CommandRequestMethod.Unknown)]
        public CommandRequestMethod Method { get; set; }

        /// <summary>
        /// Request parameters
        /// </summary>
        [JsonProperty("params")]
        public object[] Parameters { get; set; }

        /// <summary>
        /// Id to identify a response to the request
        /// </summary>
        [JsonProperty("id")]
        public long RequestId { get; set; }
    }
}
