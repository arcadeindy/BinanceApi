using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// Response to Create Listen Key Request
    /// </summary>
    public class CreateListenKeyResponse
    {
        /// <summary>
        /// Listen Key
        /// </summary>
        [JsonProperty("listenKey")]
        public string ListenKey { get; set; }
    }
}
