using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PoissonSoft.BinanceApi.Contracts;

namespace PoissonSoft.BinanceApi.Transport
{
    internal class Throttler
    {
        private readonly BinanceApiClient apiClient;
        private readonly int maxDegreeOfParallelism;
        private readonly string userFriendlyName = nameof(Throttler);

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
        public Throttler(BinanceApiClient apiClient, int maxDegreeOfParallelism = 5)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            if (this.maxDegreeOfParallelism < 1) this.maxDegreeOfParallelism = 1;

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
        /// <param name="isOrderRequest"></param>
        public void ThrottleRest(int requestWeight, bool isOrderRequest)
        {
            // TODO:
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
        /// <param name="heeders"></param>
        public void ApplyRestResponseHeaders(HttpResponseHeaders heeders)
        {
            // TODO:
        }
    }
}
