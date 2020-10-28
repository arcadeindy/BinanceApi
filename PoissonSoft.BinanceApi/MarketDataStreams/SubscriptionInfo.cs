using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    /// <summary>
    /// Subscription information
    /// </summary>
    public class SubscriptionInfo
    {
        /// <summary>
        /// Subscription Id used to unsubscribe
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Type of Data Stream
        /// </summary>
        public DataStreamType StreamType { get; set; }

        /// <summary>
        /// Stream Name used by Binance
        /// </summary>
        public string BinanceStreamName { get; set; }

        /// <summary>
        /// Trade Instrument for which data is received in this Stream
        /// It can be null if the data relates to the entire market
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Other Stream parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }
    }
}
