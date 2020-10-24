using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary>
        /// The order has been accepted by the engine.
        /// </summary>
        [EnumMember(Value = "NEW")]
        New,

        /// <summary>
        /// A part of the order has been filled.
        /// </summary>
        [EnumMember(Value = "PARTIALLY_FILLED")]
        PartiallyFilled,

        /// <summary>
        /// The order has been completed.
        /// </summary>
        [EnumMember(Value = "FILLED")]
        Filled,

        /// <summary>
        /// Currently unused
        /// </summary>
        [EnumMember(Value = "PENDING_CANCEL")]
        PendingCancel,

        /// <summary>
        /// The order was not accepted by the engine and not processed.
        /// </summary>
        [EnumMember(Value = "REJECTED")]
        Rejected,

        /// <summary>
        /// The order was canceled according to the order type's rules (e.g. LIMIT FOK orders with no fill,
        /// LIMIT IOC or MARKET orders that partially fill) or by the exchange, (e.g. orders canceled during
        /// liquidation, orders canceled during maintenance)
        /// </summary>
        [EnumMember(Value = "EXPIRED")]
        Expired,
    }
}
