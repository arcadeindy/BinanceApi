using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// Balance Update occurs during the following:
    ///   - Deposits or withdrawals from the account
    ///   - Transfer of funds between accounts (e.g.Spot to Margin)
    /// </summary>
    public class BalanceUpdatePayload
    {
        /// <summary>
        /// Тип события: balanceUpdate
        /// </summary>
        [JsonProperty("e")]
        public string EventType { get; set; }

        /// <summary>
        /// Event Time
        /// "E": 1564034571105,
        /// </summary>
        [JsonProperty("E")]
        public long EventTime { get; set; }

        /// <summary>
        /// Asset
        /// "a": "BTC",
        /// </summary>
        [JsonProperty("a")]
        public string Asset { get; set; }

        /// <summary>
        /// Balance Delta
        /// "d": "100.00000000",
        /// </summary>
        [JsonProperty("d")]
        public decimal BalanceDelta { get; set; }

        /// <summary>
        /// Clear Time
        /// "T": 1573200697068
        /// (Maybe it is a real time of the transfer produced this event) 
        /// </summary>
        [JsonProperty("T")]
        public long ClearTime { get; set; }
    }

}
