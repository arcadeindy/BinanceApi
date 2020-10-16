using System;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    internal sealed class SpotAccountApi : ISpotAccountApi, IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly RestClient client;

        public SpotAccountApi(BinanceApiClient apiClient, BinanceApiClientCredentials credentials, ILogger logger)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            client = new RestClient(logger, "https://api.binance.com/api/v3",
                new[] {EndpointSecurityType.Trade, EndpointSecurityType.UserData}, credentials, 
                this.apiClient.Throttler);
        }

        public AccountInformation GetAccountInformation()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
