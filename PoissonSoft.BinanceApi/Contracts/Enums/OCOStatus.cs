using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// OCO Status (listStatusType)
    /// </summary>
    public enum OCOStatus
    {
        /// <summary>
        /// Unknown (erroneous)
        /// </summary>
        Unknown,

        /// <summary>
        /// This is used when the ListStatus is responding to a failed action. (E.g. OrderList placement or cancellation)
        /// </summary>
        [EnumMember(Value = "RESPONSE")]
        Response,

        /// <summary>
        /// The order list has been placed or there is an update to the order list status.
        /// </summary>
        [EnumMember(Value = "EXEC_STARTED")]
        ExecStarted,

        /// <summary>
        /// The order list has finished executing and thus no longer active.
        /// </summary>
        [EnumMember(Value = "ALL_DONE")]
        AllDone,

    }
}
