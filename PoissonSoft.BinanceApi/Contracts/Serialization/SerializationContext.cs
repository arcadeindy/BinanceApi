using Newtonsoft.Json;
using NLog;

namespace PoissonSoft.BinanceApi.Contracts.Serialization
{
    internal class SerializationContext
    {
        public ILogger Logger { get; set; }

        public static ILogger GetLogger(JsonSerializer serializer)
        {
            if (serializer.Context.Context is SerializationContext context)
                return context.Logger;

            return null;
        }
    }
}
