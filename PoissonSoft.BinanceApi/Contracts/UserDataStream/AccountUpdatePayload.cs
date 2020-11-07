/* При вводе/выводе прилетает сразу два снапшота AccountUpdatePayload и BalanceUpdatePayload
Поэтому отслеживать изменения балансов целесообразно исключительно по снапшотам AccountUpdatePayload

{
  "e": "outboundAccountPosition",
  "E": 1603617640389,
  "u": 1603617640387,
  "B": [
    {
      "a": "DOGE",
      "f": 19900.00000000,
      "l": 0.00000000
    }
  ]
}

{
  "e": "balanceUpdate",
  "E": 1603617640389,
  "a": "DOGE",
  "d": -100.00000000,
  "T": 1603617640387
}

 */


using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.SpotAccount;

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
