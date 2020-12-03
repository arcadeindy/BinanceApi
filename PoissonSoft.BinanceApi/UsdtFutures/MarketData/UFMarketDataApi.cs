using System;
using System.Net.Http;
using NLog;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;
using PoissonSoft.BinanceApi.UsdtFutures.Contracts;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.UsdtFutures.MarketData
{
    internal class UFMarketDataApi: IUFMarketDataApi, IDisposable
    {
        private readonly RestClient client;

        public UFMarketDataApi(Throttler throttler, BinanceApiClientCredentials credentials, ILogger logger)
        {
            if (throttler == null) throw new ArgumentNullException(nameof(throttler));
            client = new RestClient(logger, "https://api.binance.com/api/v3",
                new[] { EndpointSecurityType.None }, credentials, throttler);

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
