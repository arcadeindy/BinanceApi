using System;
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
                var msg =
                    $"Не удалось преобразовать {JObject.Load(reader)} в тип {objectType.Name}. Ошибка: {e.Message}";
                if (defaultValue != null)
                {
                    msg += $"\n Будет использовано значение {defaultValue}";
                }
                SerializationContext.GetLogger(serializer)?.Error(msg);
                if (defaultValue == null) throw;
                return defaultValue;
            }
        }
    }
}
