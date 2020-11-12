using System;
using System.Collections.Generic;
using System.Text;
using PoissonSoft.BinanceApi.Contracts.Wallet;

namespace PoissonSoft.BinanceApi.Wallet
{
    /// <summary>
    /// Wallet REST-API
    /// </summary>
    public interface IWalletApi
    {
        /// <summary>
        /// Get information of coins (available for deposit and withdraw) for user.
        /// </summary>
        /// <param name="cacheValidityIntervalSec">Допускается возврат кешированных значений,
        /// полученных в течение указанного количества секунд назад. Если кешированные данные отсутствуют
        /// или были получены ранее указанного времени, то будут загружены актуальные данные</param>
        /// <returns></returns>
        BinanceCoinInfo[] AllCoinsInformation(int cacheValidityIntervalSec = 10 * 60);

        /// <summary>
        /// Fetch deposit address with network.
        /// If network is not send, return with default network of the coin.
        /// You can get network and isDefault in networkList in the response of <see cref="AllCoinsInformation"/>
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        DepositAddress DepositAddress(string coin, string network = null);
    }
}
