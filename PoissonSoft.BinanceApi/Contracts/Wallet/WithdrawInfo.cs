using System;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Withdraw history record
    /// </summary>
    public class WithdrawInfo
    {
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
        /// "amount":"0.00999800",
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// "applyTime": "2019-10-12 11:12:02",
        /// </summary>
        [JsonProperty("applyTime")]
        public DateTimeOffset? ApplyTime { get; set; }
        
        /// <summary>
        /// "coin": "BTC",
        /// </summary>
        [JsonProperty("coin")]
        public string Coin { get; set; }

        /// <summary>
        /// "id": "b6ae22b3aa844210a7041aee7589627c",
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// "withdrawOrderId": "WITHDRAWtest123",
        /// Will not be returned if there's no withdrawOrderId for this withdraw.
        /// </summary>
        [JsonProperty("withdrawOrderId")]
        public string WithdrawOrderId { get; set; }
        
        /// <summary>
        /// "network":"ETH",
        /// </summary>
        [JsonProperty("network")]
        public string Network { get; set; }

        /// <summary>
        /// "transferType":0,
        /// 1 for internal transfer, 0 for external transfer 
        /// </summary>
        [JsonProperty("transferType")]
        public TransferType? TransferType { get; set; }

        /// <summary>
        /// "status": 6,
        /// </summary>
        [JsonProperty("status")]
        public WithdrawStatus Status { get; set; }

        /// <summary>
        /// "txId":"0xaad4654a3234aa6118af9b4b335f5ae81c360b2394721c019b5d1e75328b09f3",
        /// </summary>
        [JsonProperty("txId")]
        public string TxId { get; set; }

    }

    /// <summary>
    /// Withdraw status
    /// </summary>
    public enum WithdrawStatus
    {
        /// <summary>
        /// Email Sent
        /// </summary>
        EmailSent = 0,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 1,

        /// <summary>
        /// Awaiting Approval
        /// </summary>
        AwaitingApproval = 2,

        /// <summary>
        /// Rejected 
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Processing 
        /// </summary>
        Processing = 4,

        /// <summary>
        /// Failure 
        /// </summary>
        Failure = 5,

        /// <summary>
        /// Completed
        /// </summary>
        Completed = 6,
    }
}
