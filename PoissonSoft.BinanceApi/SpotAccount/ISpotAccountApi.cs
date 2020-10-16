using PoissonSoft.BinanceApi.Contracts;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    /// <summary>
    /// Rest-API спотового аккаунта
    /// </summary>
    public interface ISpotAccountApi
    {
        /// <summary>
        /// Получение данных аккаунта
        /// </summary>
        /// <returns></returns>
        AccountInformation GetAccountInformation();
    }
}
