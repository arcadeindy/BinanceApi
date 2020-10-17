using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PoissonSoft.BinanceApi.Contracts.Serialization
{
    /// <summary>
    /// Конвертер из string в Enum с логгирование в случае ошибки конвертации
    /// и возможностью использования заданного значения по умолчанию для ошибочных значений
    /// </summary>
    public class StringEnumExConverter : StringEnumConverter
    {
        private readonly Enum defaultValue;
        private readonly ConcurrentDictionary<string, object> errors = new ConcurrentDictionary<string, object>();
        private DateTimeOffset clearErrorCacheTime = DateTimeOffset.UtcNow.AddMinutes(10);

        /// <summary/>
        public StringEnumExConverter(){}

        /// <summary>
        /// Создание экземпляра конвертера
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию - будет использовано в ситуациях, когда
        /// не удаётся распознать Enum</param>
        public StringEnumExConverter(Enum defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception e)
            {
                string problemItem = "<PARSING ERROR>";
                try
                {
                    problemItem = JToken.Load(reader).ToString();
                }
                catch
                {
                    // ignore
                }

                if (DateTimeOffset.UtcNow > clearErrorCacheTime)
                {
                    errors.Clear();
                    clearErrorCacheTime = DateTimeOffset.UtcNow.AddMinutes(10);
                }

                if (!errors.TryGetValue(problemItem, out _))
                {
                    var msg =
                        $"Не удалось преобразовать {problemItem} в тип {objectType.Name}. Ошибка: {e.Message}";
                    if (defaultValue != null)
                    {
                        msg += $"\n Будет использовано значение {defaultValue}";
                    }
                    SerializationContext.GetLogger(serializer)?.Error(msg);

                    errors.TryAdd(problemItem, null);
                }
                
                if (defaultValue == null) throw;
                return defaultValue;
            }
        }
    }
}
