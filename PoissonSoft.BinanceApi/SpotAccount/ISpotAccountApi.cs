using PoissonSoft.BinanceApi.Contracts;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    /// <summary>
    /// Rest-API спотового аккаунта
    /// </summary>
    public interface ISpotAccountApi
    {
        /// <summary>
        /// Get all open orders on a symbol. Careful when accessing this with no symbol.
        /// Weight: 1 for a single symbol; 40 when the symbol parameter is omitted
        /// </summary>
        /// <param name="symbol">If the symbol is not sent, orders for all symbols will be returned in an array.</param>
        /// <returns></returns>
        BinanceOrder[] GetCurrentOpenOrders(string symbol);

        /// <summary>
        /// Получение данных аккаунта
        /// </summary>
        /// <returns></returns>
        AccountInformation GetAccountInformation();
    }
}
