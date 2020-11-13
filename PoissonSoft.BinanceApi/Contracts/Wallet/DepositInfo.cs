using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Deposit history record
    /// </summary>
    public class DepositInfo
    {
        /// <summary>
        /// "amount":"0.00999800",
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// "coin": "BTC",
        /// </summary>
        [JsonProperty("coin")]
        public string Coin { get; set; }

        /// <summary>
        /// "network":"ETH",
        /// </summary>
        [JsonProperty("network")]
        public string Network { get; set; }

        /// <summary>
        /// "status":1,
        /// 0: pending, 6: credited but cannot withdraw, 1: success
        /// </summary>
        [JsonProperty("status")]
        public DepositStatus Status { get; set; }

        /// <summary>
        /// "address":"0x788cabe9236ce061e5a892e1a59395a81fc8d62c",
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// "addressTag":"",
        /// </summary>
        [JsonProperty("addressTag")]
        public string AddressTag { get; set; }

        /// <summary>
        /// "txId":"0xaad4654a3234aa6118af9b4b335f5ae81c360b2394721c019b5d1e75328b09f3",
        /// </summary>
        [JsonProperty("txId")]
        public string TxId { get; set; }

        /// <summary>
        /// "insertTime":1599621997000,
        /// </summary>
        [JsonProperty("insertTime")]
        public long InsertTime { get; set; }

        /// <summary>
        /// "transferType":0,
        /// </summary>
        [JsonProperty("transferType")]
        public TransferType TransferType { get; set; }

        /// <summary>
        /// "confirmTimes":"12/12"
        /// </summary>
        [JsonProperty("confirmTimes")]
        public string ConfirmTimes { get; set; }
    }

    /// <summary>
    /// Deposit status
    /// </summary>
    public enum DepositStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Credited but cannot withdraw
        /// </summary>
        Credited = 6,

        /// <summary>
        /// Success
        /// </summary>
        Success = 1,
    }
}
