using System;
using System.Collections.Generic;
using System.Net.Http;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Contracts.SpotAccount;
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

        public BinanceOrder NewOrder(NewOrderRequest request, bool isHighPriority)
        {
            return client.MakeRequest<BinanceOrder>(new RequestParameters(HttpMethod.Post, "order", 1)
            {
                IsHighPriority = isHighPriority,
                IsOrderRequest = true,
                PassAllParametersInQueryString = true,
                Parameters = RequestParameters.GenerateParametersFromObject(request)
            });
        }

        public BinanceOrder[] CurrentOpenOrders(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return client.MakeRequest<BinanceOrder[]>(new RequestParameters(HttpMethod.Get, "openOrders", 40)
                {
                    IsOrderRequest = true
                });
            }

            return client.MakeRequest<BinanceOrder[]>(new RequestParameters(HttpMethod.Get, "openOrders", 1)
            {
                IsOrderRequest = true,
                Parameters = new Dictionary<string, string>
                {
                    ["symbol"] = symbol
                }
            });
        }

        public AccountInformation AccountInformation()
        {
            return client.MakeRequest<AccountInformation>(new RequestParameters(HttpMethod.Get, "account", 5));
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
