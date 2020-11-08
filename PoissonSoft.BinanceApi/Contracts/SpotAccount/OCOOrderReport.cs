using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Report on cancellation of the OCO order
    /// </summary>
    public class OCOOrderReport
    {
        /// <summary>
        /// "orderListId": 0
        /// </summary>
        [JsonProperty("orderListId")]
        public long OrderListId { get; set; }

        /// <summary>
        /// Contingency Type
        /// "contingencyType": "OCO",  
        /// </summary>
        [JsonProperty("contingencyType")]
        [JsonConverter(typeof(StringEnumExConverter), ContingencyType.Unknown)]
        public ContingencyType ContingencyType { get; set; }

        /// <summary>
        /// List Status Type
        /// "listStatusType": "ALL_DONE",
        /// </summary>
        [JsonProperty("listStatusType")]
        [JsonConverter(typeof(StringEnumExConverter), OCOStatus.Unknown)]
        public OCOStatus ListStatusType { get; set; }

        /// <summary>
        /// List Order Status
        /// "listOrderStatus": "ALL_DONE",, 
        /// </summary>
        [JsonProperty("listOrderStatus")]
        [JsonConverter(typeof(StringEnumExConverter), OCOOrderStatus.Unknown)]
        public OCOOrderStatus ListOrderStatus { get; set; }

        /// <summary>
        /// Used to uniquely identify cancellation request
        /// "listClientOrderId": "C3wyj4WVEktd7u9aVBRXcN",
        /// </summary>
        [JsonProperty("listClientOrderId")]
        public string ClientOrderId { get; set; }

        /// <summary>
        /// "transactionTime": 1574040868128,
        /// </summary>
        [JsonProperty("transactionTime")]
        public long TransactionTime { get; set; }

        /// <summary>
        /// "symbol": "LTCBTC",
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// "orders": [...]
        /// </summary>
        [JsonProperty("orders")]
        public OCOOrderLeg[] Orders { get; set; }

        /// <summary>
        /// "orderReports": [...]
        /// </summary>
        [JsonProperty("orderReports")]
        public OrderReport[] OrderReports { get; set; }
    }


    /// <summary>
    /// Short definition of OCO-order leg
    /// </summary>
    public class OCOOrderLeg
    {
        /// <summary>
        /// "symbol": "LTCBTC",
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// "orderId": 1,
        /// </summary>
        [JsonProperty("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        /// "clientOrderId": "461cPg51vQjV3zIMOXNz39"
        /// </summary>
        [JsonProperty("clientOrderId")]
        public string ClientOrderId { get; set; }
    }
}
