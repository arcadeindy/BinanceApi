using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NLog;
using PoissonSoft.BinanceApi.Contracts.Wallet;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.Wallet
{
    internal sealed class WalletApi : IWalletApi, IDisposable
    {
        private readonly RestClient sApiClient;
        //private readonly RestClient wApiClient;

        public WalletApi(BinanceApiClient apiClient, BinanceApiClientCredentials credentials, ILogger logger)
        {
            if (apiClient == null) throw new ArgumentNullException(nameof(apiClient));
            sApiClient = new RestClient(logger, "https://api.binance.com/sapi/v1",
                new[] { EndpointSecurityType.UserData }, credentials,
                apiClient.Throttler);
            //wApiClient = new RestClient(logger, "https://api.binance.com/wapi/v3",
            //    new[] { EndpointSecurityType.UserData }, credentials,
            //    apiClient.Throttler);

            coinsInfoCache = new SimpleCache<BinanceCoinInfo[]>(LoadCoinsInformation, logger, "CoinsInfoCache", 
                data => data.Select(x => (BinanceCoinInfo)x.Clone()).ToArray());
        }

        private readonly SimpleCache<BinanceCoinInfo[]> coinsInfoCache;
        public BinanceCoinInfo[] AllCoinsInformation(int cacheValidityIntervalSec = 600)
        {
            return coinsInfoCache.GetValue(TimeSpan.FromSeconds(cacheValidityIntervalSec));
        }

        public WithdrawResponse Withdraw(WithdrawRequest request)
        {
            return sApiClient.MakeRequest<WithdrawResponse>(
                new RequestParameters(HttpMethod.Post, "capital/withdraw/apply", 1)
                {
                    PassAllParametersInQueryString = true,
                    Parameters = RequestParameters.GenerateParametersFromObject(request)
                });
        }

        public DepositInfo[] DepositHistory(DepositHistoryRequest request)
        {
            return sApiClient.MakeRequest<DepositInfo[]>(
                new RequestParameters(HttpMethod.Get, "capital/deposit/hisrec", 1)
                {
                    Parameters = RequestParameters.GenerateParametersFromObject(request)
                });
        }

        public WithdrawInfo[] WithdrawHistory(WithdrawHistoryRequest request)
        {
            return sApiClient.MakeRequest<WithdrawInfo[]>(
                new RequestParameters(HttpMethod.Get, "capital/withdraw/history", 1)
                {
                    Parameters = RequestParameters.GenerateParametersFromObject(request)
                });
        }

        public DepositAddress DepositAddress(string coin, string network = null)
        {
            var request = new RequestParameters(HttpMethod.Get,
                "capital/deposit/address", 1)
            {
                Parameters = new Dictionary<string, string>
                {
                    ["coin"] = coin
                }
            };
            if (!string.IsNullOrWhiteSpace(network))
            {
                request.Parameters["network"] = network;
            }
            return sApiClient.MakeRequest<DepositAddress>(request);
        }

        private BinanceCoinInfo[] LoadCoinsInformation()
        {
            var request = new RequestParameters(HttpMethod.Get, "capital/config/getall", 1);
            request.Parameters = new Dictionary<string, string> { { "recvWindow", "30000" } };
            return sApiClient.MakeRequest<BinanceCoinInfo[]>(request);
        }

        public void Dispose()
        {
            sApiClient?.Dispose();
            //wApiClient?.Dispose();
        }
    }
}
