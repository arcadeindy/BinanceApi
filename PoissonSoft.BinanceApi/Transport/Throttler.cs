using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.Transport
{
    internal class Throttler: IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly int maxDegreeOfParallelism;
        private readonly int highPriorityFeedsCount;
        private readonly string userFriendlyName = nameof(Throttler);
        private readonly WaitablePool syncPool;
        
        // "Стоимость" одного бала в миллисекундах для каждого из параллельно исполняемых запросов.
        // Т.е. если в конкретном потоке (одном из всех maxDegreeOfParallelism параллельных) выполняется запрос
        // с весом 1 балл, то этот поток не должен проводить новых запросов в течение requestWeightCostInMs миллисекунд
        private int weightUnitCostInMs;

        /// <summary>
        /// Включение/выключение автоматического обновления актуальных лимитов.
        /// По умолчанию авто-обновление включено
        /// </summary>
        public bool AutoUpdateLimits { get; set; } = true;

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="apiClient"></param>
        /// <param name="maxDegreeOfParallelism">Максимальное допустимое количество параллельно выполняемых запросов</param>
        /// <param name="highPriorityFeedsCount">Количество feeds, выделяемое под исполнение запросов с высоким приоритетом</param>
        public Throttler(BinanceApiClient apiClient, int maxDegreeOfParallelism = 5, int highPriorityFeedsCount = 1)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            if (this.maxDegreeOfParallelism < 1) this.maxDegreeOfParallelism = 1;

            if (highPriorityFeedsCount >= this.maxDegreeOfParallelism)
                highPriorityFeedsCount = this.maxDegreeOfParallelism - 1;


            ApplyLimits(new []
            {
                new RateLimit
                {
                    RateLimitType = RateLimitType.RequestWeight,
                    IntervalUnit = RateLimitUnit.Minute,
                    IntervalNum = 1,
                    Limit = 1200,
                }, 
                new RateLimit
                {
                    RateLimitType = RateLimitType.Orders,
                    IntervalUnit = RateLimitUnit.Second,
                    IntervalNum = 10,
                    Limit = 100,
                },
                new RateLimit
                {
                    RateLimitType = RateLimitType.Orders,
                    IntervalUnit = RateLimitUnit.Day,
                    IntervalNum = 1,
                    Limit = 200_000,
                },
            });

            syncPool = new WaitablePool(this.maxDegreeOfParallelism, highPriorityFeedsCount);
        }

        /// <summary>
        /// Применить актуальные лимиты
        /// </summary>
        /// <param name="limits"></param>
        public void ApplyLimits(RateLimit[] limits)
        {
            // В данной реализации учитываем пока только лимит на вес запросов с одного IP.
            // лимит количества ордеров не отслеживаем

            var weightPerSecondLimits = limits?.Where(x =>
                    x.RateLimitType == RateLimitType.RequestWeight &&
                    x.IntervalUnit > 0 &&
                    x.IntervalNum > 0 &&
                    x.Limit > 0)
                .Select(x => (double) x.Limit / ((int) x.IntervalUnit * x.IntervalNum))
                .Where(x => x > 0)
                .ToArray();

            double minWeightPerSecondLimit;
            if (weightPerSecondLimits?.Any() == true)
            {
                minWeightPerSecondLimit = weightPerSecondLimits.Min();
            }
            else
            {
                minWeightPerSecondLimit = 20;
                apiClient.Logger.Error(
                    $"{userFriendlyName}. Среди переданных в метод {nameof(ApplyLimits)} лимитов " +
                    $"не обнаружено ни одного валидного лимита типа {RateLimitType.RequestWeight}. " +
                    $"Будет использовано значение по умолчанию ({minWeightPerSecondLimit}) " +
                    "для лимита весов запросов в секунду");
            }

            var costMs = (int)Math.Ceiling(1000 / minWeightPerSecondLimit) * maxDegreeOfParallelism;

            Interlocked.Exchange(ref weightUnitCostInMs, costMs);
        }

        /// <summary>
        /// REST-request throttling
        /// </summary>
        /// <param name="requestWeight"></param>
        /// <param name="highPriority"></param>
        /// <param name="isOrderRequest"></param>
        public void ThrottleRest(int requestWeight, bool highPriority, bool isOrderRequest)
        {
            var dt = DateTimeOffset.UtcNow;
            var locker = syncPool.Wait(highPriority);
            locker.UnlockAfterMs(requestWeight * weightUnitCostInMs);
            var waitTime = (DateTimeOffset.UtcNow - dt).TotalSeconds;
            if (waitTime > 5)
            {
                apiClient.Logger.Warn($"{userFriendlyName}. Время ожидания тротлинга составило {waitTime:F0} секунд. " +
                                      "Возможно, следует оптимизировать прикладные алгоритмы с целью сокращения количества запросов");
            }

            if (AutoUpdateLimits && ssUpdateLimits.Check())
            {
                Task.Run(UpdateLimits);
            }
        }

        /// <summary>
        /// Apply response headers.
        /// IP Limits
        ///     Every request will contain X-MBX-USED-WEIGHT-(intervalNum)(intervalLetter) in the response headers which
        ///     has the current used weight for the IP for all request rate limiters defined.
        /// Order Rate Limits
        ///     Every successful order response will contain a X-MBX-ORDER-COUNT-(intervalNum)(intervalLetter) header which
        ///     has the current order count for the account for all order rate limiters defined.
        /// </summary>
        /// <param name="headers"></param>
        public void ApplyRestResponseHeaders(HttpResponseHeaders headers)
        {
            // TODO:
            // Not implemented yet
        }

        private readonly SimpleScheduler ssUpdateLimits = new SimpleScheduler(TimeSpan.FromMinutes(10));
        private readonly object syncUpdateLimits = new object();
        private void UpdateLimits()
        {
            lock (syncUpdateLimits)
            {
                if (!ssUpdateLimits.Check()) return;
                try
                {
                    var exchangeInfo = apiClient.MarketDataApi.GetExchangeInfo();
                    ApplyLimits(exchangeInfo.RateLimits);
                    ssUpdateLimits.Done();
                }
                catch (Exception e)
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Exception when limit update\n{e}");
                }
            }
        }

        public void Dispose()
        {
            syncPool?.Dispose();
        }
    }
}
