using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using NLog;
using PoissonSoft.BinanceApi.Contracts.SpotAccount;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    internal sealed class SpotAccountApi : ISpotAccountApi, IDisposable
    {
        private readonly RestClient client;

        public SpotAccountApi(BinanceApiClient apiClient, BinanceApiClientCredentials credentials, ILogger logger)
        {
            if (apiClient == null) throw new ArgumentNullException(nameof(apiClient));
            client = new RestClient(logger, "https://api.binance.com/api/v3",
                new[] {EndpointSecurityType.Trade, EndpointSecurityType.UserData}, credentials, 
                apiClient.Throttler);
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

        public OrderReport CancelOrder(CancelOrderRequest request, bool isHighPriority)
        {
            return client.MakeRequest<OrderReport>(new RequestParameters(HttpMethod.Delete, "order", 1)
            {
                IsHighPriority = isHighPriority,
                IsOrderRequest = true,
                PassAllParametersInQueryString = true,
                Parameters = RequestParameters.GenerateParametersFromObject(request)
            });
        }

        public OrderReportsContainer CancelAllOrdersOnSymbol(string binanceSymbol, bool isHighPriority)
        {
            var respArray = client.MakeRequest<JArray>(new RequestParameters(HttpMethod.Delete, "openOrders", 1)
            {
                IsHighPriority = isHighPriority,
                IsOrderRequest = true,
                PassAllParametersInQueryString = true,
                Parameters = new Dictionary<string, string>
                {
                    ["symbol"] = binanceSymbol
                }
            });

            var orders = new List<OrderReport>();
            var ocoOrders = new List<OCOOrderReport>();

            foreach (var jObject in respArray.OfType<JObject>())
            {
                if (jObject.ContainsKey("contingencyType"))
                {
                    ocoOrders.Add(jObject.ToObject<OCOOrderReport>());
                }
                else
                {
                    orders.Add(jObject.ToObject<OrderReport>());
                }
            }

            return new OrderReportsContainer
            {
                Orders = orders,
                OCOOrders = ocoOrders
            };
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

        public BinanceTrade[] AccountTradeList(TradeListRequest request, bool isHighPriority)
        {
            return client.MakeRequest<BinanceTrade[]>(new RequestParameters(HttpMethod.Get, "myTrades", 5)
            {
                IsHighPriority = isHighPriority,
                IsOrderRequest = true,
                PassAllParametersInQueryString = true,
                Parameters = RequestParameters.GenerateParametersFromObject(request)
            });
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
