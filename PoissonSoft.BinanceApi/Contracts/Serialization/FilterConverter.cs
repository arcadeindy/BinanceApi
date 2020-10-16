using System;
using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoissonSoft.BinanceApi.Contracts.Filters;

namespace PoissonSoft.BinanceApi.Contracts.Serialization
{
    /// <summary>
    /// Конвертер фильтров
    /// </summary>
    public class FilterConverter : JsonConverter
    {

        private readonly ConcurrentDictionary<string, object> errors = new ConcurrentDictionary<string, object>();
        private DateTimeOffset clearErrorCacheTime = DateTimeOffset.UtcNow.AddMinutes(10);

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => false;

        private static readonly Type[] availableTypes = {typeof(SymbolFilter), typeof(ExchangeFilter)};

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!availableTypes.Contains(objectType))
            {
                SerializationContext.GetLogger(serializer)?.Error(
                    $"Некорректное значение целевого типа {objectType.Name} в методе " +
                    $"{nameof(FilterConverter)}.{nameof(ReadJson)}");
                return null;
            }

            var jObject = JObject.Load(reader);
            object baseObject;
            try
            {
                baseObject = jObject.ToObject(objectType);
            }
            catch (Exception e)
            {
                if (DateTimeOffset.UtcNow > clearErrorCacheTime)
                {
                    errors.Clear();
                    clearErrorCacheTime = DateTimeOffset.UtcNow.AddMinutes(10);
                }

                if (errors.TryGetValue($"{jObject}", out _)) return CreateUnknownInstance(objectType);

                SerializationContext.GetLogger(serializer)?.Error(
                    $"Исключение при десериализации объекта {jObject} в тип {objectType.Name}." +
                    $"Ошибка: {e.Message}\n" +
                    "Будет использован неизвестный (Unknown) тип фильтра");

                errors.TryAdd($"{jObject}", null);

                return CreateUnknownInstance(objectType);
            }

            var concreteObject = CreateConcreteInstance(baseObject);

            if (concreteObject == null) return CreateUnknownInstance(objectType);

            serializer.Populate(jObject.CreateReader(), concreteObject);
            return concreteObject;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        
        private object CreateUnknownInstance(Type objectType)
        {
            if (objectType == typeof(SymbolFilter))
            {
                return new SymbolFilter { FilterType = SymbolFilterType.Unknown };
            }
            
            if (objectType == typeof(ExchangeFilter))
            {
                return new ExchangeFilter { FilterType = ExchangeFilterType.Unknown };
            }

            return null;
        }

        private object CreateConcreteInstance(object baseObject)
        {
            if (baseObject is SymbolFilter symbolFilter)
            {
                switch (symbolFilter.FilterType)
                {
                    case SymbolFilterType.PriceFilter: return new SymbolFilterPriceFilter();
                    case SymbolFilterType.PercentPrice: return new SymbolFilterPercentPrice();
                    case SymbolFilterType.LotSize: return new SymbolFilterLotSize();
                    case SymbolFilterType.MinNotional: return new SymbolFilterMinNotional();
                    case SymbolFilterType.IcebergParts: return new SymbolFilterIcebergParts();
                    case SymbolFilterType.MarketLotSize: return new SymbolFilterMarketLotSize();
                    case SymbolFilterType.MaxNumOrders: return new SymbolFilterMaxNumOrders();
                    case SymbolFilterType.MaxNumAlgoOrders: return new SymbolFilterMaxNumAlgoOrders();
                    case SymbolFilterType.MaxNumIcebergOrders: return new SymbolFilterMaxNumIcebergOrders();
                    case SymbolFilterType.MaxPosition: return  new SymbolFilterMaxPosition();
                }
            }
            else if (baseObject is ExchangeFilter exchangeFilter)
            {
                switch (exchangeFilter.FilterType)
                {
                    case ExchangeFilterType.ExchangeMaxNumOrders: return new ExchangeFilterMaxNumOrders();
                    case ExchangeFilterType.ExchangeMaxNumAlgoOrders: return new ExchangeFilterMaxNumAlgoOrders();
                }
            }

            return null;
        }
    }
}
