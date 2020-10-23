using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// Is sent any time an account balance has changed and contains the assets that
    /// were possibly changed by the event that generated the balance change.
    /// </summary>
    public class AccountUpdatePayload
    {
        /// <summary>
        /// Тип события: outboundAccountPosition
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
        /// Time of last account update
        /// "u": 1564034571073,
        /// </summary>
        [JsonProperty("u")]
        public long LastAccountUpdateTime { get; set; }

        /// <summary>
        /// Array of balances which were possibly changed by the event that generated the balance change
        /// </summary>
        [JsonProperty("B")]
        public BalancePayload[] ChangedBalances { get; set; }
    }

    /// <summary>
    /// Balance
    /// </summary>
    public class BalancePayload
    {
        /// <summary>
        /// Asset
        /// "a": "DOGE",
        /// </summary>
        [JsonProperty("a")]
        public string Asset { get; set; }

        /// <summary>
        /// Free amount
        /// "f": "19000.00000000",
        /// </summary>
        [JsonProperty("f")]
        public decimal Free { get; set; }

        /// <summary>
        /// Locked amount
        /// "l": "1000.00000000"
        /// </summary>
        [JsonProperty("l")]
        public decimal Locked { get; set; }

        /// <summary>
        /// Convert to main balance class
        /// </summary>
        /// <returns></returns>
        public Balance ToBalance()
        {
            return new Balance
            {
                Asset = Asset,
                Free = Free,
                Locked = Locked
            };
        }
    }
}
