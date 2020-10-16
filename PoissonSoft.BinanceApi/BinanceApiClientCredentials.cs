using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi
{
    /// <summary>
    /// Binance and proxy credentials
    /// </summary>
    public class BinanceApiClientCredentials
    {
        /// <summary>
        /// Api key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Secret Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Proxy server address (optional)
        /// If this parameter is not set direct connection will be used
        /// </summary>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Proxy server credentials in format LOGIN@PASSWORD
        /// </summary>
        public string ProxyCredentials { get; set; }
    }
}
