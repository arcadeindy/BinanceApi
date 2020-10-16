using System;
using NLog;

namespace PoissonSoft.BinanceApi.Utils
{
    internal class SimpleCache<T> where T: ICloneable
    {
        private readonly Func<T> loadDataDelegate;
        private readonly ILogger logger;

        private T cachedValue;
        private DateTimeOffset dataUploadTime = DateTimeOffset.MinValue;
        private readonly object syncObj = new object();

        public string Name { get; }

        public SimpleCache(Func<T> loadDataDelegate, ILogger logger, string name = null)
        {
            this.loadDataDelegate = loadDataDelegate ?? throw new ArgumentNullException(nameof(loadDataDelegate));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Name = string.IsNullOrWhiteSpace(name)
                ? $"SimpleCache of {typeof(T).Name}"
                : name;
        }

        public T GetValue(TimeSpan cacheValidityInterval)
        {
            if (cachedValue != null && dataUploadTime > DateTimeOffset.UtcNow - cacheValidityInterval)
                return (T)cachedValue?.Clone();

            lock (syncObj)
            {
                if (cachedValue != null && dataUploadTime > DateTimeOffset.UtcNow - cacheValidityInterval)
                    return (T)cachedValue?.Clone();

                try
                {
                    cachedValue = loadDataDelegate();
                    dataUploadTime = DateTimeOffset.UtcNow;
                }
                catch (Exception e)
                {
                    logger.Error($"{Name}. Исключение при попытке загрузить данные\n{e}");
                }
            }

            return (T)cachedValue?.Clone();
        }
    }
}
