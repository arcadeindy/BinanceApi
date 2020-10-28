using System;
using System.Collections.Generic;
using System.Text;
using PoissonSoft.BinanceApi.Contracts.MarketDataStream;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    internal class MarketStreamsManager: IMarketStreamsManager, IDisposable
    {

        public MarketStreamsManager(BinanceApiClient apiClient, BinanceApiClientCredentials credentials)
        {
            // TODO:
        }


        public SubscriptionInfo SubscribePartialBookDepth(string symbol, int levelsCount, int updateSpeedMs, Action<PartialBookDepthPayload> callbackAction)
        {
            throw new NotImplementedException();
        }

        public bool Unsubscribe(long subscriptionId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
