using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Order execution type
    /// </summary>
    public enum ExecutionType
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary>
        /// The order has been accepted into the engine.
        /// </summary>
        [EnumMember(Value = "NEW")]
        New,

        /// <summary>
        /// The order has been canceled by the user.
        /// </summary>
        [EnumMember(Value = "CANCELED")]
        Canceled,

        /// <summary>
        /// (currently unused)
        /// </summary>
        [EnumMember(Value = "REPLACED")]
        Replaced,

        /// <summary>
        /// The order has been rejected and was not processed.
        /// (This is never pushed into the User Data Stream)
        /// </summary>
        [EnumMember(Value = "REJECTED")]
        Rejected,

        /// <summary>
        /// Part of the order or all of the order's quantity has filled.
        /// </summary>
        [EnumMember(Value = "TRADE")]
        Trade,

        /// <summary>
        /// The order was canceled according to the order type's rules (e.g. LIMIT FOK orders with no fill,
        /// LIMIT IOC or MARKET orders that partially fill) or by the exchange, (e.g. orders canceled during
        /// liquidation, orders canceled during maintenance)
        /// </summary>
        [EnumMember(Value = "EXPIRED")]
        Expired,
    }
}
