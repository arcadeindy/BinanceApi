using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Exceptions;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.Transport
{
    internal class Throttler: IDisposable
    {
        private readonly ILogger logger;
        private readonly Func<RateLimit[]> rateLimitProvider;

        private readonly string userFriendlyName = nameof(Throttler);
        private readonly WaitablePool syncPool;
        private readonly WaitablePool syncPoolWs;
        
        // "Стоимость" одного бала в миллисекундах для каждого из параллельно исполняемых REST-запросов.
        // Т.е. если в конкретном потоке (одном из всех MaxDegreeOfParallelism параллельных) выполняется запрос
        // с весом 1 балл, то этот поток не должен проводить новых запросов в течение requestWeightCostInMs миллисекунд
        private int weightUnitCostInMs;

        // "Стоимость" одного бала в миллисекундах для каждого из параллельно исполняемых WebSocket-запросов.
        private readonly int wsWeightUnitCostInMs;

        // Значение по умолчанию для времени приостановки запросов после превышения лимита (в секундах)
        private int defaultRetryAfterSec = 60;

        // Время (UTC), до которого приостановлены все запросы в связи с превышением лимита
        private object rateLimitPausedTime = DateTimeOffset.MinValue;

        /// <summary>
        /// Включение/выключение автоматического обновления актуальных лимитов.
        /// По умолчанию авто-обновление включено
        /// </summary>
        public bool AutoUpdateLimits { get; set; } = true;

        /// <summary>
        /// Максимальное количество параллельно выполняемых запросов
        /// </summary>
        public int MaxDegreeOfParallelism { get; }

        /// <summary>
        /// Количество Feeds, выделяемых под высоко-приоритетные запросы
        /// Значение должно быть строго меньше чем <see cref="MaxDegreeOfParallelism"/>
        /// </summary>
        public int HighPriorityFeedsCount { get; }

        /// <summary>
        /// Create instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="rateLimitProvider">Метод, позволяющих получить актуальные лимиты запросов</param>
        /// <param name="maxDegreeOfParallelism">Максимальное допустимое количество параллельно выполняемых запросов</param>
        /// <param name="highPriorityFeedsCount">Количество feeds, выделяемое под исполнение запросов с высоким приоритетом</param>
        public Throttler(ILogger logger, Func<RateLimit[]> rateLimitProvider, int maxDegreeOfParallelism = 5, int highPriorityFeedsCount = 1)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rateLimitProvider = rateLimitProvider ?? throw new ArgumentNullException(nameof(rateLimitProvider));
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            if (MaxDegreeOfParallelism < 1) MaxDegreeOfParallelism = 1;

            HighPriorityFeedsCount = highPriorityFeedsCount;
            if (HighPriorityFeedsCount >= MaxDegreeOfParallelism)
                HighPriorityFeedsCount = MaxDegreeOfParallelism - 1;


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

            syncPool = new WaitablePool(MaxDegreeOfParallelism, highPriorityFeedsCount);

            const int WS_MaxDegreeOfParallelism = 5;
            // WebSocket connections have a limit of 5 incoming messages per second.
            const int WS_RequestLimitPerSecond = 5;
            syncPoolWs = new WaitablePool(WS_MaxDegreeOfParallelism, 0);
            wsWeightUnitCostInMs = WS_MaxDegreeOfParallelism * 1000 / WS_RequestLimitPerSecond;
        }

        /// <summary>
        /// Применить актуальные лимиты
        /// </summary>
        /// <param name="limits"></param>
        public void ApplyLimits(RateLimit[] limits)
        {
            // В данной реализации учитываем пока только лимит на вес запросов с одного IP.
            // лимит количества ордеров не отслеживаем
            var validRateLimits = limits?.Where(x =>
                x.RateLimitType == RateLimitType.RequestWeight &&
                x.IntervalUnit > 0 &&
                x.IntervalNum > 0 &&
                x.Limit > 0).ToArray();

            var weightPerSecondLimits = validRateLimits
                ?.Select(x => (double) x.Limit / ((int) x.IntervalUnit * x.IntervalNum))
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
                logger.Error(
                    $"{userFriendlyName}. Среди переданных в метод {nameof(ApplyLimits)} лимитов " +
                    $"не обнаружено ни одного валидного лимита типа {RateLimitType.RequestWeight}. " +
                    $"Будет использовано значение по умолчанию ({minWeightPerSecondLimit}) " +
                    "для лимита весов запросов в секунду");
            }

            var costMs = (int)Math.Ceiling(1000 / minWeightPerSecondLimit) * MaxDegreeOfParallelism;

            Interlocked.Exchange(ref weightUnitCostInMs, costMs);

            var timeFrames = validRateLimits
                ?.Select(x => (int) x.IntervalUnit * x.IntervalNum)
                .Where(x => x > 0)
                .ToArray();
            var maxTimeFrame = timeFrames?.Any() == true ? timeFrames.Max() : 60;
            Interlocked.Exchange(ref defaultRetryAfterSec, maxTimeFrame);
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
                logger.Warn($"{userFriendlyName}. Время ожидания тротлинга REST-запроса составило {waitTime:F0} секунд. " +
                                      "Возможно, следует оптимизировать прикладные алгоритмы с целью сокращения количества запросов");
            }

            if (AutoUpdateLimits && ssUpdateLimits.Check())
            {
                Task.Run(UpdateLimits);
            }

            // Здесь не используем Interlocked для чтения rateLimitPausedTime по следующим соображениям:
            // - кривое значение будет прочитано в исключительно редких случаях, при этом возможно будет пропущен запрос, который следовало пресечь, или
            //   остановлен запрос, который следовало пропустить. Это не является большой проблемой
            // - в подавляющем большинстве случаев запрос будет пропускаться, при этом лишняя задержка на Interlocked операцию здесь выглядит совсем лишней
            if (DateTimeOffset.UtcNow < (DateTimeOffset)rateLimitPausedTime)
                throw new RequestRateLimitBreakingException($"All requests banned until {(DateTimeOffset)rateLimitPausedTime}");
        }

        /// <summary>
        /// WebSocket send message throttling
        /// 
        /// </summary>
        /// <param name="requestWeight"></param>
        public void ThrottleWs(int requestWeight)
        {
            var dt = DateTimeOffset.UtcNow;
            var locker = syncPool.Wait(false);
            locker.UnlockAfterMs(requestWeight * wsWeightUnitCostInMs);
            var waitTime = (DateTimeOffset.UtcNow - dt).TotalSeconds;
            if (waitTime > 5)
            {
                logger.Warn($"{userFriendlyName}. Время ожидания тротлинга WebSocket-запроса составило {waitTime:F0} секунд. " +
                                      "Возможно, следует оптимизировать прикладные алгоритмы с целью сокращения количества запросов");
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

        /// <summary>
        /// HTTP 429 return code is used when breaking a request rate limit.
        /// HTTP 418 return code is used when an IP has been auto-banned for continuing to send requests after receiving 429 codes.
        /// A Retry-After header is sent with a 418 or 429 responses and will give the number of seconds required to wait, in the case of a 429,
        /// to prevent a ban, or, in the case of a 418, until the ban is over.
        /// </summary>
        /// <param name="retryAfter">Время приостановки приёма запросов</param>
        public void StopAllRequestsDueToRateLimit(TimeSpan? retryAfter)
        {
            var timeout = retryAfter ?? TimeSpan.FromSeconds(defaultRetryAfterSec);

            var pausedTime = DateTimeOffset.UtcNow + timeout;

            Interlocked.Exchange(ref rateLimitPausedTime, pausedTime);
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
                    //var exchangeInfo = apiClient.MarketDataApi.GetExchangeInfo();
                    //ApplyLimits(exchangeInfo?.RateLimits);
                    ApplyLimits(rateLimitProvider());
                    ssUpdateLimits.Done();
                }
                catch (Exception e)
                {
                    logger.Error($"{userFriendlyName}. Exception when limit update\n{e}");
                }
            }
        }

        public void Dispose()
        {
            syncPool?.Dispose();
            syncPoolWs?.Dispose();
        }
    }
}
