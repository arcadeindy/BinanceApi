using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Request object used to create a new Binance order
    /// </summary>
    public class NewOrderRequest
    {
        /// <summary>
        /// "symbol": "LTCBTC",
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Side
        /// "side": "BUY",
        /// </summary>
        [JsonProperty("side")]
        [JsonConverter(typeof(StringEnumExConverter), OrderSide.Unknown)]
        public OrderSide Side { get; set; }

        /// <summary>
        /// Order type
        /// "type": "LIMIT",
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumExConverter), OrderType.Unknown)]
        public OrderType Type { get; set; }

        /// <summary>
        /// Time in force (optional)
        /// "timeInForce": "GTC",
        /// </summary>
        [JsonProperty("timeInForce")]
        [JsonConverter(typeof(StringEnumExConverter), Enums.TimeInForce.Unknown)]
        public TimeInForce? TimeInForce { get; set; }

        /// <summary>
        /// Base asset quantity (optional)
        /// </summary>
        [JsonProperty("quantity")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? QuantityBase { get; set; }

        /// <summary>
        /// Quote asset quantity (optional)
        /// </summary>
        [JsonProperty("quoteOrderQty")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? QuantityQuote { get; set; }

        /// <summary>
        /// Order price (optional)
        /// </summary>
        [JsonProperty("price")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? Price { get; set; }

        /// <summary>
        /// Client Id of new order (optional)
        /// A unique id among open orders. Automatically generated if not sent.
        /// </summary>
        [JsonProperty("newClientOrderId")]
        public string NewClientOrderId { get; set; }

        /// <summary>
        /// Stop price (optional)
        /// Used with STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
        /// </summary>
        [JsonProperty("stopPrice")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? StopPrice { get; set; }

        /// <summary>
        /// Iceberg Quantity (optional)
        /// Used with LIMIT, STOP_LOSS_LIMIT, and TAKE_PROFIT_LIMIT to create an iceberg order.
        /// </summary>
        [JsonProperty("icebergQty")]
        [JsonConverter(typeof(DecimalToStringConverter))]
        public decimal? IcebergQuantity { get; set; }

        /// <summary>
        /// Response type of new order request (optional)
        /// Set the response JSON. ACK, RESULT, or FULL; MARKET and LIMIT order types default to FULL,
        /// all other orders default to ACK.
        /// </summary>
        [JsonProperty("newOrderRespType")]
        [JsonConverter(typeof(StringEnumExConverter))]
        public NewOrderResponseType? NewOrderResponseType { get; set; }
    }

    /// <summary>
    /// Response type of new order request
    /// </summary>
    public enum NewOrderResponseType
    {
        /// <summary>
        /// Unknown (erroneous) type
        /// </summary>
        Unknown,

        /// <summary/>
        [EnumMember(Value = "ACK")]
        Ack,

        /// <summary/>
        [EnumMember(Value = "RESULT")]
        Result,

        /// <summary/>
        [EnumMember(Value = "FULL")]
        Full,
    }
}
