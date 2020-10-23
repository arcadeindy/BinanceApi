using System;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.UserDataStreams;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    internal class SpotDataCollector: ISpotDataCollector, IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly object startLock = new object();

        public SpotDataCollector(BinanceApiClient apiClient)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public void Start()
        {
            lock (startLock)
            {
                if (IsStarted) return;

                // TODO:

                if (apiClient.SpotDataStream.Status != UserDataStreamStatus.Active)
                {
                    apiClient.SpotDataStream.Open();
                }
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool IsStarted { get; }

        public AccountInformation AccountInformation { get; }

        public void Dispose()
        {
            // TODO:
        }
    }
}
