using System;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// Фильтр, задающий ограничения на количество ордеров по всей бирже
    /// </summary>
    public class ExchangeFilter : ICloneable
    {
        /// <summary>
        /// Тип фильтра
        /// </summary>
        [JsonProperty("filterType")]
        [JsonConverter(typeof(StringEnumExConverter), ExchangeFilterType.Unknown)]
        public ExchangeFilterType FilterType { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return CreateInstanceForClone();
        }

        /// <summary>
        /// Создание экземпляра объекта, возвращаемого методом <see cref="Clone"/>
        /// </summary>
        /// <returns></returns>
        protected virtual ExchangeFilter CreateInstanceForClone()
        {
            return new ExchangeFilter
            {
                FilterType = FilterType
            };
        }
    }
}
