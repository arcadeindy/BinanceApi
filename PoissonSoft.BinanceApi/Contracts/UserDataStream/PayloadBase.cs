using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// Base class of User Data Payload
    /// </summary>
    public class PayloadBase
    {
        /// <summary>
        /// Тип события
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; }
    }
}
