using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Типы ордеров
    /// https://www.binance.com/en/support/articles/360033779452-Types-of-Order
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary>
        /// Limit Order
        /// </summary>
        [EnumMember(Value = "LIMIT")]
        Limit,

        /// <summary>
        /// Market Order
        /// </summary>
        [EnumMember(Value = "MARKET")]
        Market,

        /// <summary>
        /// Stop Loss Order
        /// </summary>
        [EnumMember(Value = "STOP_LOSS")]
        StopLoss,

        /// <summary>
        /// Stop Loss Limit Order
        /// </summary>
        [EnumMember(Value = "STOP_LOSS_LIMIT")]
        StopLossLimit,

        /// <summary>
        /// Take Profit Order
        /// </summary>
        [EnumMember(Value = "TAKE_PROFIT")]
        TakeProfit,

        /// <summary>
        /// Take Profit Limit Order
        /// </summary>
        [EnumMember(Value = "TAKE_PROFIT_LIMIT")]
        TakeProfitLimit,

        /// <summary>
        /// Limit Maker Order
        /// </summary>
        [EnumMember(Value = "LIMIT_MAKER")]
        LimitMaker,

        #region [Дополнительные типы ордеров для USDT-Futures]

        /// <summary>
        /// Stop Order
        /// </summary>
        [EnumMember(Value = "LIMIT_MAKER")]
        Stop,

        /// <summary>
        /// Stop Market Order
        /// </summary>
        [EnumMember(Value = "STOP_MARKET")]
        StopMarket,

        /// <summary>
        /// TakeProfit Market Order
        /// </summary>
        [EnumMember(Value = "TAKE_PROFIT_MARKET")]
        TakeProfitMarket,

        /// <summary>
        /// Trailing Stop Market Order
        /// </summary>
        [EnumMember(Value = "TRAILING_STOP_MARKET")]
        TrailingStopMarket,

        #endregion
    }
}
