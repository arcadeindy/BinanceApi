using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Withdraw request
    /// </summary>
    public class WithdrawRequest
    {
        /// <summary>
        /// Coin to withdraw (REQUIRED)
        /// </summary>
        [JsonProperty("coin")]
        public string Coin { get; set; }

        /// <summary>
        /// Client id for withdraw (optional)
        /// </summary>
        [JsonProperty("withdrawOrderId")]
        public string WithdrawOrderId { get; set; }

        /// <summary>
        /// Network (optional)
        /// If network not send, return with default network of the coin.
        /// </summary>
        [JsonProperty("network")]
        public string Network { get; set; }

        /// <summary>
        /// Wallet address (REQUIRED)
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Secondary address identifier for coins like XRP, XMR etc. (optional)
        /// </summary>
        [JsonProperty("addressTag")]
        public string AddressTag { get; set; }

        /// <summary>
        /// Withdrawing amount of the coin (REQUIRED)
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// TransactionFeeFlag (optional)
        /// When making internal transfer, true for returning the fee to the destination account;
        /// false for returning the fee back to the departure account. Default false.
        /// </summary>
        [JsonProperty("transactionFeeFlag")]
        public bool TransactionFeeFlag { get; set; }

        /// <summary>
        /// Description of the address. Space in name should be encoded into %20. (optional)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
