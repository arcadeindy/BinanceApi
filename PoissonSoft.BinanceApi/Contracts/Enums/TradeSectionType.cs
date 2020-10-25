using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Тип торговой секции
    /// </summary>
    public enum TradeSectionType
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
        /// </summary>
        Unknown,

        /// <summary/>
        [EnumMember(Value = "SPOT")]
        Limit,

        /// <summary/>
        [EnumMember(Value = "MARGIN")]
        Margin,

        /// <summary/>
        [EnumMember(Value = "FUTURES")]
        Futures,

        /// <summary/>
        [EnumMember(Value = "LEVERAGED")]
        Leveraged,
    }
}
