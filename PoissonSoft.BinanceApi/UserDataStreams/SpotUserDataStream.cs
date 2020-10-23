using System;
using System.Collections.Generic;
using System.Net.Http;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Transport.Rest;

namespace PoissonSoft.BinanceApi.UserDataStreams
{

    /// <summary>
    /// User Data Stream для спотового аккаунта
    /// </summary>
    public class SpotUserDataStream : UserDataStream
    {
        private readonly RestClient client;

        /// <inheritdoc />
        public SpotUserDataStream(BinanceApiClient apiClient, BinanceApiClientCredentials credentials) : base(apiClient.Logger, credentials)
        {
            StreamType = UserDataStreamType.Spot;

            client = new RestClient(apiClient.Logger, "https://api.binance.com/api/v3",
                new[] { EndpointSecurityType.UserStream }, credentials,
                apiClient.Throttler);
        }

        /// <inheritdoc />
        protected override string CreateListenKey()
        {
            var response =
                client.MakeRequest<CreateListenKeyResponse>(
                    new RequestParameters(HttpMethod.Post, "userDataStream",1));

            if (string.IsNullOrWhiteSpace(response.ListenKey)) 
                throw new Exception("Server returned empty Listen Key");

            return response.ListenKey;
        }

        /// <inheritdoc />
        protected override void KeepAliveListenKey(string key)
        {
            client.MakeRequest<EmptyResponse>(
                new RequestParameters(HttpMethod.Put, "userDataStream", 1)
                {
                    Parameters = new Dictionary<string, string>
                    {
                        ["listenKey"] = key
                    },
                    PassAllParametersInQueryString = true
                });
        }

        /// <inheritdoc />
        protected override void CloseListenKey(string key)
        {
            client.MakeRequest<EmptyResponse>(
                new RequestParameters(HttpMethod.Delete, "userDataStream", 1)
                {
                    Parameters = new Dictionary<string, string>
                    {
                        ["listenKey"] = key
                    }
                });
        }

        private bool disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                client?.Dispose();
            }

            disposed = true;

            base.Dispose(disposing);
        }
    }
}
