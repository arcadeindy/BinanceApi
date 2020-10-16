using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
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

    /// <summary>
    /// Тип лимита
    /// </summary>
    public enum RateLimitType
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
        /// </summary>
        Unknown,

        /// <summary>
        /// Лимит на суммарный вес запросов с одного IP
        /// </summary>
        [EnumMember(Value = "REQUEST_WEIGHT")]
        RequestWeight,

        /// <summary>
        /// Лимит на количество ордеров
        /// </summary>
        [EnumMember(Value = "ORDERS")]
        Orders,
    }

    /// <summary>
    /// Единицы измерения временного интервала для лимита
    /// </summary>
    public enum RateLimitUnit
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
        /// </summary>
        Unknown = 0,

        /// <summary/>
        [EnumMember(Value = "SECOND")]
        Second = 1,

        /// <summary/>
        [EnumMember(Value = "MINUTE")]
        Minute = Second * 60,

        /// <summary/>
        [EnumMember(Value = "HOUR")]
        Hour = Minute * 60,

        /// <summary/>
        [EnumMember(Value = "DAY")]
        Day = Hour * 24,

    }
}
