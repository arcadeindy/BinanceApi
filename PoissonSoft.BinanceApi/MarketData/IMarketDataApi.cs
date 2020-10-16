using PoissonSoft.BinanceApi.Contracts;

namespace PoissonSoft.BinanceApi.MarketData
{
    /// <summary>
    /// Rest-API для получение рыночных данных
    /// </summary>
    public interface IMarketDataApi
    {
        /// <summary>
        /// Current exchange trading rules and symbol information
        /// </summary>
        /// <param name="cacheValidityIntervalSec">Допускается возврат кешированных значений,
        /// полученных в течение указанного количества секунд назад. Если кешированные данные отсутствуют
        /// или были получены ранее указанного времени, то будут загружены актуальные данные</param>
        /// <returns></returns>
        ExchangeInfo GetExchangeInfo(int cacheValidityIntervalSec = 30 * 60);
    }
}
