using System;
using System.Net.Http;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.MarketData
{
    internal class MarketDataApi: IMarketDataApi, IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly RestClient client;

        public MarketDataApi(BinanceApiClient apiClient, BinanceApiClientCredentials credentials, ILogger logger)
        {

            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            client = new RestClient(logger, "https://api.binance.com/api/v3",
                new[] { EndpointSecurityType.None }, credentials,
                this.apiClient.Throttler);

            exchangeInfoCache = new SimpleCache<ExchangeInfo>(LoadExchangeInfo, logger);
        }

        private readonly SimpleCache<ExchangeInfo> exchangeInfoCache;
        public ExchangeInfo GetExchangeInfo(int cacheValidityIntervalSec = 1800)
        {
            return exchangeInfoCache.GetValue(TimeSpan.FromSeconds(cacheValidityIntervalSec));
        }
        private ExchangeInfo LoadExchangeInfo()
        {
            return client.MakeRequest<ExchangeInfo>(new RequestParameters(HttpMethod.Get, "exchangeInfo", 1));
        }

        public void Dispose()
        {
            client?.Dispose();
        }

    }
}
