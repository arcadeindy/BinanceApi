using System;

namespace PoissonSoft.BinanceApi.Transport
{
    internal class Throttler
    {
        private readonly BinanceApiClient apiClient;

        public Throttler(BinanceApiClient apiClient)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        // TODO:
    }
}
