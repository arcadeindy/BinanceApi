using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts
{
    /// <summary>
    /// Типы ордеров
    /// https://www.binance.com/en/support/articles/360033779452-Types-of-Order
    /// </summary>
    public enum BinanceOrderType
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
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
    }
}
