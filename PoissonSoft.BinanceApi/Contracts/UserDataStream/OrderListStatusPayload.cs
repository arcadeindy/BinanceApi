using Newtonsoft.Json;

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
        // TODO: 
    }
}
