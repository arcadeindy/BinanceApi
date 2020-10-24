using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.UserDataStream
{
    /// <summary>
    /// If the order is an OCO, an event will be displayed named ListStatus in addition to the executionReport event.
    /// https://academy.binance.com/ru/articles/what-is-an-oco-order
    /// </summary>
    public class OrderListStatusPayload
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
        /// OrderListId
        /// "g": 2,
        /// </summary>
        [JsonProperty("g")]
        public long OrderListId { get; set; }

        /// <summary>
        /// Contingency Type
        /// "c": "OCO",  
        /// </summary>
        [JsonProperty("c")]
        [JsonConverter(typeof(StringEnumExConverter), ContingencyType.Unknown)]
        public ContingencyType ContingencyType { get; set; }

        /// <summary>
        /// List Status Type
        /// "l": "EXEC_STARTED",  
        /// </summary>
        [JsonProperty("l")]
        [JsonConverter(typeof(StringEnumExConverter), OCOStatus.Unknown)]
        public OCOStatus ListStatusType { get; set; }

        /// <summary>
        /// List Order Status
        /// "L": "EXECUTING", 
        /// </summary>
        [JsonProperty("L")]
        [JsonConverter(typeof(StringEnumExConverter), OCOOrderStatus.Unknown)]
        public OCOOrderStatus ListOrderStatus { get; set; }

        /// <summary>
        /// List Reject Reason
        /// "r": "NONE",
        /// </summary>
        [JsonProperty("r")]
        public string RejectReason { get; set; }

        /// <summary>
        /// List Client Order ID
        /// "C": "F4QN4G8DlFATFlIUQ0cjdD", 
        /// </summary>
        [JsonProperty("C")]
        public string ListClientOrderId { get; set; }

        /// <summary>
        /// Transaction time
        /// "T": 1499405658657,
        /// </summary>
        [JsonProperty("T")]
        public long TransactionTime { get; set; }

        /// <summary>
        /// Array of orders
        /// "O": [...],
        /// </summary>
        [JsonProperty("O")]
        public OrderListStatusPayloadItem[] Orders { get; set; }
    }

    /// <summary>
    /// Элемент списка ордеров в OrderListStatusPayload
    /// </summary>
    public class OrderListStatusPayloadItem
    {
        /// <summary>
        /// Symbol
        /// "s": "ETHBTC",
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; }

        /// <summary>
        /// Order ID
        /// "i": 17, 
        /// </summary>
        [JsonProperty("i")]
        public long OrderId { get; set; }

        /// <summary>
        /// Client order ID
        /// "c": "mUvoqJxFIILMdfAW5iGSOW",
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; }
    }
}
