using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// Событие, информирующее об изменении ордера
    /// </summary>
    public class OrderExecutionReportPayload
    {
        /// <summary>
        /// Тип события
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
        /// Symbol
        /// "s": "ETHBTC",
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; }

        /// <summary>
        /// Client order ID
        /// "c": "mUvoqJxFIILMdfAW5iGSOW",
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; }

        /// <summary>
        /// Side
        /// "S": "BUY", 
        /// </summary>
        [JsonProperty("S")]
        [JsonConverter(typeof(StringEnumExConverter), OrderSide.Unknown)]
        public OrderSide Side { get; set; }

        /// <summary>
        /// Order type
        /// "o": "LIMIT",
        /// </summary>
        [JsonProperty("o")]
        [JsonConverter(typeof(StringEnumExConverter), Enums.OrderType.Unknown)]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Time in force
        /// "f": "GTC",
        /// </summary>
        [JsonProperty("f")]
        [JsonConverter(typeof(StringEnumExConverter), Enums.TimeInForce.Unknown)]
        public TimeInForce TimeInForce { get; set; }

        /// <summary>
        /// Order quantity
        /// "q": "1.00000000",  
        /// </summary>
        [JsonProperty("q")]
        public decimal OrderQuantity { get; set; }

        /// <summary>
        /// Order price
        /// "p": "0.10264410",   
        /// </summary>
        [JsonProperty("p")]
        public decimal OrderPrice { get; set; }

        /// <summary>
        /// Stop price
        /// "P": "0.00000000",  
        /// </summary>
        [JsonProperty("P")]
        public decimal StopPrice { get; set; }

        /// <summary>
        /// Iceberg quantity
        /// "F": "0.00000000",  
        /// </summary>
        [JsonProperty("F")]
        public decimal IcebergQuantity { get; set; }

        // TODO:
    }
}
