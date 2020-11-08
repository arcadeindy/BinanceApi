using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Cancel order report
    /// </summary>
    public class OrderReport
    {
        /// <summary>
        /// "symbol": "LTCBTC",
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Client ID of the order which was cancelled
        /// "origClientOrderId": "myOrder1",
        /// </summary>
        [JsonProperty("origClientOrderId")]
        public string OriginalClientOrderId { get; set; }

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
        /// Used to uniquely identify cancellation request
        /// "clientOrderId": "cancelMyOrder1",
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }

        /// <summary>
        /// "price": "0.1",
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// "origQty": "1.0",
        /// </summary>
        [JsonProperty("origQty")]
        public decimal OriginalQuantity { get; set; }

        /// <summary>
        /// "executedQty": "0.0",
        /// </summary>
        [JsonProperty(PropertyName = "executedQty")]
        public decimal ExecutedQuantity { get; set; }

        /// <summary>
        /// "cummulativeQuoteQty": "0.0",
        /// </summary>
        [JsonProperty(PropertyName = "cummulativeQuoteQty")]
        public decimal CumulativeQuoteQuantity { get; set; }

        /// <summary>
        /// "status": "CANCELED",
        /// </summary>
        [JsonConverter(typeof(StringEnumExConverter), OrderStatus.Unknown)]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Time in force
        /// "timeInForce": "GTC",
        /// </summary>
        [JsonProperty("timeInForce")]
        [JsonConverter(typeof(StringEnumExConverter), TimeInForce.Unknown)]
        public TimeInForce TimeInForce { get; set; }

        /// <summary>
        /// Order type
        /// "type": "LIMIT",
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumExConverter), OrderType.Unknown)]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Side
        /// "side": "BUY",
        /// </summary>
        [JsonProperty("side")]
        [JsonConverter(typeof(StringEnumExConverter), OrderSide.Unknown)]
        public OrderSide Side { get; set; }

        /// <summary>
        /// Stop price
        /// "stopPrice": "0.0", 
        /// </summary>
        [JsonProperty("stopPrice")]
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Iceberg quantity
        /// "icebergQty": "0.0", 
        /// </summary>
        [JsonProperty("icebergQty")]
        public decimal IcebergQuantity { get; set; }
    }
}
