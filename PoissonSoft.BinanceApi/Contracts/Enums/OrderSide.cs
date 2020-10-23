using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Направление сделки
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary>
        /// BUY
        /// </summary>
        [EnumMember(Value = "BUY")]
        Buy,

        /// <summary>
        /// SELL
        /// </summary>
        [EnumMember(Value = "SELL")]
        Sell,
    }
}
