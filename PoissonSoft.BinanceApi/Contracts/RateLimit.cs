using System;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts
{
    /// <summary>
    /// Лимит
    /// </summary>
    public class RateLimit : ICloneable
    {

        /// <summary>
        /// Тип лимита
        /// </summary>
        [JsonProperty("rateLimitType")]
        [JsonConverter(typeof(StringEnumExConverter), RateLimitType.Unknown)]
        public RateLimitType RateLimitType { get; set; }

        /// <summary>
        /// Единица измерения временного интервала, на который распространяется лимит
        /// (см. константы INTERVAL_*)
        /// </summary>
        [JsonProperty("interval")]
        [JsonConverter(typeof(StringEnumExConverter), RateLimitUnit.Unknown)]
        public RateLimitUnit IntervalUnit { get; set; }

        /// <summary>
        /// Размер временного интервала, на который распространяется лимит, в единицах,
        /// заданных в поле <see cref="IntervalUnit"/>
        /// </summary>
        [JsonProperty("intervalNum")]
        public int IntervalNum { get; set; }

        /// <summary>
        /// Размер лимита (максимальное допустимое количество операций за данный период)
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new RateLimit
            {
                RateLimitType = RateLimitType,
                IntervalUnit = IntervalUnit,
                IntervalNum = IntervalNum,
                Limit = Limit
            };
        }
    }
}
