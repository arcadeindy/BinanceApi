using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Enums
{
    /// <summary>
    /// Статус торгового инструмента
    /// </summary>
    public enum InstrumentStatus
    {
        /// <summary>
        /// Неизвестный (ошибочный) статус
        /// </summary>
        Unknown,

        /// <summary>
        /// PRE_TRADING
        /// </summary>
        [EnumMember(Value = "PRE_TRADING")]
        PreTrading,

        /// <summary>
        /// Торги активны
        /// </summary>
        [EnumMember(Value = "TRADING")]
        Trading,
        
        /// <summary>
        /// POST_TRADING
        /// </summary>
        [EnumMember(Value = "POST_TRADING")]
        PostTrading,

        /// <summary>
        /// END_OF_DAY
        /// </summary>
        [EnumMember(Value = "END_OF_DAY")]
        EndOfDay,

        /// <summary>
        /// HALT
        /// </summary>
        [EnumMember(Value = "HALT")]
        Halt,

        /// <summary>
        /// AUCTION_MATCH
        /// </summary>
        [EnumMember(Value = "AUCTION_MATCH")]
        AuctionMatch,

        /// <summary>
        /// Торги приостановлены
        /// </summary>
        [EnumMember(Value = "BREAK")]
        Break,
    }
}