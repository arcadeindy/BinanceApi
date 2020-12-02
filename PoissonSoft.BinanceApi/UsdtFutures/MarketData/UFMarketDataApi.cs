using System;
using System.Net.Http;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;
using PoissonSoft.BinanceApi.UsdtFutures.Contracts;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.UsdtFutures.MarketData
{
    internal class UFMarketDataApi: IUFMarketDataApi, IDisposable
    {
        private readonly UFBinanceApiClient apiClient;
        private readonly RestClient client;

        public UFMarketDataApi(UFBinanceApiClient apiClient, BinanceApiClientCredentials credentials, ILogger logger)
        {

            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            client = new RestClient(logger, "https://api.binance.com/api/v3",
                new[] { EndpointSecurityType.None }, credentials,
                this.apiClient.Throttler);

            exchangeInfoCache = new SimpleCache<UFExchangeInfo>(LoadExchangeInfo, logger);
        }

        private readonly SimpleCache<UFExchangeInfo> exchangeInfoCache;
        public UFExchangeInfo GetExchangeInfo(int cacheValidityIntervalSec = 1800)
        {
            return exchangeInfoCache.GetValue(TimeSpan.FromSeconds(cacheValidityIntervalSec));
        }
        private UFExchangeInfo LoadExchangeInfo()
        {
            return client.MakeRequest<UFExchangeInfo>(new RequestParameters(HttpMethod.Get, "exchangeInfo", 1));
        }

        public void Dispose()
        {
            client?.Dispose();
        }

    }
}
