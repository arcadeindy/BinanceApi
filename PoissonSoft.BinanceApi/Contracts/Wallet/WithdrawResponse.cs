using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Response to withdraw request
    /// </summary>
    public class WithdrawResponse
    {
        /// <summary>
        /// "id": "b6ae22b3aa844210a7041aee7589627c",
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
