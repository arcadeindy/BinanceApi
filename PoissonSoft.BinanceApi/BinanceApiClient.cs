using System;
using NLog;
using PoissonSoft.BinanceApi.MarketData;
using PoissonSoft.BinanceApi.MarketDataStreams;
using PoissonSoft.BinanceApi.SpotAccount;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.UserDataStreams;

namespace PoissonSoft.BinanceApi
{
    /// <summary>
    /// Клиент для работы с API Binance
    /// </summary>
    public sealed class BinanceApiClient : IDisposable
    {
        private readonly BinanceApiClientCredentials credentials;

        internal ILogger Logger { get; }

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="logger"></param>
        public BinanceApiClient(BinanceApiClientCredentials credentials, ILogger logger)
        {
            Logger = logger;
            this.credentials = credentials;
            Throttler = new Throttler(this);
            
            spotDataStream = new SpotUserDataStream(this, credentials);
            spotDataCollector = new SpotDataCollector(this);
            marketDataApi = new MarketDataApi(this, credentials, logger);
            spotAccountApi = new SpotAccountApi(this, credentials, logger);

            marketStreamsManager = new MarketStreamsManager(this, credentials);
        }

        /// <summary>
        /// В режиме отладке логгируется больше событий
        /// </summary>
        public bool IsDebug { get; set; } = false;

        internal Throttler Throttler { get; }

        /// <summary>
        /// Rest-API для получение рыночных данных
        /// </summary>
        public IMarketDataApi MarketDataApi => marketDataApi;
        private readonly MarketDataApi marketDataApi;

        /// <summary>
        /// Менеджер подписок на рыночные данные
        /// </summary>
        public IMarketStreamsManager MarketStreamsManager => marketStreamsManager;
        private readonly MarketStreamsManager marketStreamsManager;

        /// <summary>
        /// API спотового аккаунта
        /// </summary>
        public ISpotAccountApi SpotAccountApi => spotAccountApi;
        private readonly SpotAccountApi spotAccountApi;

        /// <summary>
        /// User Data Stream для спотового аккаунта
        /// </summary>
        public IUserDataStream SpotDataStream => spotDataStream;
        private readonly SpotUserDataStream spotDataStream;

        /// <summary>
        /// Сборщик актуальных данных по Spot-аккаунту
        /// (балансы, ордеры, трейды)
        /// </summary>
        public ISpotDataCollector SpotDataCollector => spotDataCollector;
        private readonly SpotDataCollector spotDataCollector;

        /// <inheritdoc />
        public void Dispose()
        {
            
            if (spotDataCollector?.IsStarted == true) spotDataCollector.Stop();
            spotDataCollector?.Dispose();

            if (spotDataStream?.Status == UserDataStreamStatus.Active) spotDataStream.Close();
            spotDataStream?.Dispose();

            marketStreamsManager?.Dispose();

            marketDataApi?.Dispose();
            spotAccountApi?.Dispose();
            
            Throttler?.Dispose();
        }
    }
}
