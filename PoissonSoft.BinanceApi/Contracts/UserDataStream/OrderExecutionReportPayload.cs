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

        /// <summary>
        /// OrderListId
        /// "g": -1, 
        /// </summary>
        [JsonProperty("g")]
        public long OrderListId { get; set; }

        /// <summary>
        /// Original client order ID; This is the ID of the order being canceled
        /// "C": "", 
        /// </summary>
        [JsonProperty("C")]
        public string OriginalClientOrderId { get; set; }

        /// <summary>
        /// Current execution type
        /// "x": "NEW",
        /// </summary>
        [JsonProperty("x")]
        [JsonConverter(typeof(StringEnumExConverter), Enums.ExecutionType.Unknown)]
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        /// Current order status
        /// "X": "NEW",
        /// </summary>
        [JsonProperty("X")]
        [JsonConverter(typeof(StringEnumExConverter), Enums.OrderStatus.Unknown)]
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Order reject reason; will be an error code.
        /// "r": "NONE",
        /// </summary>
        [JsonProperty("r")]
        public string RejectReason { get; set; }

        /// <summary>
        /// Order ID
        /// "i": 4293153,
        /// </summary>
        [JsonProperty("i")]
        public long OrderId { get; set; }

        /// <summary>
        /// Last executed quantity
        /// "l": "0.00000000",  
        /// </summary>
        [JsonProperty("l")]
        public decimal LastExecutedQuantity { get; set; }

        /// <summary>
        /// Cumulative filled quantity
        /// "z": "0.00000000",  
        /// </summary>
        [JsonProperty("z")]
        public decimal CumulativeFilledQuantity { get; set; }

        /// <summary>
        /// Last executed price
        /// "L": "0.00000000",  
        /// </summary>
        [JsonProperty("L")]
        public decimal LastExecutedPrice { get; set; }

        /// <summary>
        /// Commission amount
        /// "n": "0",  
        /// </summary>
        [JsonProperty("n")]
        public decimal CommissionAmount { get; set; }

        /// <summary>
        /// Commission asset
        /// "N": null,  
        /// </summary>
        [JsonProperty("N")]
        public string CommissionAsset { get; set; }

        /// <summary>
        /// Transaction time
        /// "T": 1499405658657,
        /// </summary>
        [JsonProperty("T")]
        public long TransactionTime { get; set; }

        /// <summary>
        /// Trade ID
        /// "t": -1, 
        /// </summary>
        [JsonProperty("t")]
        public long TradeId { get; set; }

        /// <summary>
        /// Is the order on the book?
        /// "w": true, 
        /// </summary>
        [JsonProperty("w")]
        public bool IsOrderOnBook { get; set; }

        /// <summary>
        /// Is this trade the maker side?
        /// "m": false,
        /// </summary>
        [JsonProperty("m")]
        public bool IsMakerTrade { get; set; }

        /// <summary>
        /// Order creation time
        /// "O": 1499405658657, 
        /// </summary>
        [JsonProperty("O")]
        public long OrderCreationTime { get; set; }

        /// <summary>
        /// Cumulative quote asset transacted quantity
        /// "Z": "0.00000000", 
        /// </summary>
        [JsonProperty("Z")]
        public decimal CumulativeTransactedQuote { get; set; }

        /// <summary>
        /// Last quote asset transacted quantity (i.e. lastPrice * lastQty)
        /// "Y": "0.00000000", 
        /// </summary>
        [JsonProperty("Y")]
        public decimal LastTransactedQuote { get; set; }

        /// <summary>
        /// Quote Order Qty
        /// "Q": "0.00000000" 
        /// </summary>
        [JsonProperty("Q")]
        public decimal OrderQuantityQuote { get; set; }
    }
}
