using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Trade
    /// </summary>
    public class BinanceTrade
    {
        /// <summary>
        /// "symbol": "LTCBTC",
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// "id": 28457,
        /// </summary>
        [JsonProperty("id")]
        public long TradeId { get; set; }

        /// <summary>
        /// "orderId": 1,
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// "orderListId": -1, // Unless part of an OCO, the value will always be -1
        /// </summary>
        [JsonProperty("orderListId")]
        public long OrderListId { get; set; }

        /// <summary>
        /// "price": "0.1",
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// "qty": "12.00000000",
        /// </summary>
        [JsonProperty("qty")]
        public decimal QuantityBase { get; set; }

        /// <summary>
        /// "quoteQty": "48.000012",
        /// </summary>
        [JsonProperty("quoteQty")]
        public decimal QuantityQuote { get; set; }

        /// <summary>
        /// "commission": "4.00000000",
        /// </summary>
        [JsonProperty("commission")]
        public decimal Commission { get; set; }

        /// <summary>
        /// "commissionAsset": "USDT"
        /// </summary>
        [JsonProperty("commissionAsset")]
        public string CommissionAsset { get; set; }

        /// <summary>
        /// "time": 1499865549590,
        /// </summary>
        [JsonProperty("time")]
        public long Time { get; set; }

        /// <summary>
        /// "isBuyer": true,
        /// </summary>
        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }

        /// <summary>
        /// "isMaker": false,
        /// </summary>
        [JsonProperty("isMaker")]
        public bool IsMaker { get; set; }

        /// <summary>
        /// "isBestMatch": true
        /// </summary>
        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }
    }

}
