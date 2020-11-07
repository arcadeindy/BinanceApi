using System;
using System.Globalization;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Serialization
{
    /// <summary>
    /// Конвертер, позволяющий при сериализации получать значения decimal в виде строк
    /// </summary>
    public class DecimalToStringConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
        }
    }
}
