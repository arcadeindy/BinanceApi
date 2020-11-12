using System;
using NLog;

namespace PoissonSoft.BinanceApi.Utils
{
    internal class SimpleCache<T> where T: ICloneable
    {
        private readonly Func<T> loadDataDelegate;
        private readonly Func<T, T> cloneDataDelegate;
        private readonly ILogger logger;

        private T cachedValue;
        private DateTimeOffset dataUploadTime = DateTimeOffset.MinValue;
        private readonly object syncObj = new object();

        public string Name { get; }

        public SimpleCache(Func<T> loadDataDelegate, ILogger logger, string name = null, Func<T, T> cloneDataDelegate = null)
        {
            this.loadDataDelegate = loadDataDelegate ?? throw new ArgumentNullException(nameof(loadDataDelegate));
            this.cloneDataDelegate = cloneDataDelegate;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Name = string.IsNullOrWhiteSpace(name)
                ? $"SimpleCache of {typeof(T).Name}"
                : name;
        }

        public T GetValue(TimeSpan cacheValidityInterval)
        {
            if (cachedValue != null && dataUploadTime > DateTimeOffset.UtcNow - cacheValidityInterval)
                return CloneData(cachedValue);

            lock (syncObj)
            {
                if (cachedValue != null && dataUploadTime > DateTimeOffset.UtcNow - cacheValidityInterval)
                    return CloneData(cachedValue);

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

            return CloneData(cachedValue);
        }

        public T CloneData(T data)
        {
            if (cloneDataDelegate != null) return cloneDataDelegate(data);
            return (T)data?.Clone();
        }
    }
}
