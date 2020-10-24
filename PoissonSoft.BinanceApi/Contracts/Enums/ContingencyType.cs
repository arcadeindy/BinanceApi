using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Contingency Type
    /// </summary>
    public enum ContingencyType
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary>
        /// OCO
        /// </summary>
        [EnumMember(Value = "OCO")]
        OCO,

    }
}
