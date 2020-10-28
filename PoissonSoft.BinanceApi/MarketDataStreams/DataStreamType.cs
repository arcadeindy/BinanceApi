using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    /// <summary>
    /// Type of Data Stream
    /// </summary>
    public enum DataStreamType
    {
        /// <summary>
        /// Unknown (erroneous)
        /// </summary>
        Unknown,

        /// <summary>
        /// The Aggregate Trade Streams push trade information that is aggregated for a single taker order.
        /// </summary>
        AggregateTrade,

        /// <summary>
        /// The Trade Streams push raw trade information; each trade has a unique buyer and seller.
        /// </summary>
        Trade,

        /// <summary>
        /// The Kline/Candlestick Stream push updates to the current klines/candlestick every second.
        /// </summary>
        Candlestick,

        /// <summary>
        /// 24hr rolling window mini-ticker statistics. These are NOT the statistics of the UTC day,
        /// but a 24hr rolling window for the previous 24hrs.
        /// </summary>
        IndividualSymbolMiniTicker,

        /// <summary>
        /// 24hr rolling window mini-ticker statistics for all symbols that changed in an array.
        /// These are NOT the statistics of the UTC day, but a 24hr rolling window for the previous 24hrs.
        /// Note that only tickers that have changed will be present in the array.
        /// </summary>
        AllMarketMiniTickers,

        /// <summary>
        /// 24hr rollwing window ticker statistics for a single symbol. These are NOT the statistics of the
        /// UTC day, but a 24hr rolling window for the previous 24hrs.
        /// </summary>
        IndividualSymbolTicker,

        /// <summary>
        /// 24hr rolling window ticker statistics for all symbols that changed in an array. These are NOT
        /// the statistics of the UTC day, but a 24hr rolling window for the previous 24hrs. Note that only
        /// tickers that have changed will be present in the array.
        /// </summary>
        AllMarketTickers,

        /// <summary>
        /// Pushes any update to the best bid or ask's price or quantity in real-time for a specified symbol.
        /// </summary>
        IndividualSymbolBookTicker,

        /// <summary>
        /// Pushes any update to the best bid or ask's price or quantity in real-time for all symbols.
        /// </summary>
        AllBookTickers,

        /// <summary>
        /// Top bids and asks, Valid are 5, 10, or 20.
        /// </summary>
        PartialBookDepthStreams,

        /// <summary>
        /// Order book price and quantity depth updates used to locally manage an order book.
        /// </summary>
        DiffDepth
    }
}
