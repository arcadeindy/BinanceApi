using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Contracts.SpotAccount;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    /// <summary>
    /// Rest-API спотового аккаунта
    /// </summary>
    public interface ISpotAccountApi
    {
        /// <summary>
        /// Send in a new order.
        /// </summary>
        /// <returns></returns>
        BinanceOrder NewOrder(NewOrderRequest request, bool isHighPriority);

        /// <summary>
        /// Cancel an active order.
        /// </summary>
        /// <returns></returns>
        OrderReport CancelOrder(CancelOrderRequest request, bool isHighPriority);

        /// <summary>
        /// Cancels all active orders on a symbol.
        /// This includes OCO orders.
        /// </summary>
        /// <returns></returns>
        OrderReportsContainer CancelAllOrdersOnSymbol(string binanceSymbol, bool isHighPriority);

        /// <summary>
        /// Get all open orders on a symbol. Careful when accessing this with no symbol.
        /// Weight: 1 for a single symbol; 40 when the symbol parameter is omitted
        /// </summary>
        /// <param name="symbol">If the symbol is not sent, orders for all symbols will be returned in an array.</param>
        /// <returns></returns>
        BinanceOrder[] CurrentOpenOrders(string symbol);

        /// <summary>
        /// Получение данных аккаунта
        /// </summary>
        /// <returns></returns>
        AccountInformation AccountInformation();
    }
}
