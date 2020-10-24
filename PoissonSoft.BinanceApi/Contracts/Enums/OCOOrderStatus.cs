using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// OCO Order Status (listOrderStatus)
    /// </summary>
    public enum OCOOrderStatus
    {
        /// <summary>
        /// Unknown (erroneous)
        /// </summary>
        Unknown,

        /// <summary>
        /// Either an order list has been placed or there is an update to the status of the list.
        /// </summary>
        [EnumMember(Value = "EXECUTING")]
        Executing,

        /// <summary>
        /// An order list has completed execution and thus no longer active.
        /// </summary>
        [EnumMember(Value = "ALL_DONE")]
        AllDone,

        /// <summary>
        /// The List Status is responding to a failed action either during order placement or order canceled.
        /// </summary>
        [EnumMember(Value = "REJECT")]
        Reject,

    }
}
