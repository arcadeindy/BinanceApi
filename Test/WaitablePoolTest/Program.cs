using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.Utils;

namespace WaitablePoolTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var syncPool = new WaitablePool(5);

            var callStats = new ConcurrentDictionary<int, DateTimeOffset>();

            for (int i = 0; i < 100; i++)
            {
                new Thread(() =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    var locker = syncPool.Wait();
                    var dt = DateTimeOffset.UtcNow;
                    Console.WriteLine(
                        callStats.TryGetValue(locker.Id, out var lastCall)
                            ? $"Locker {locker.Id} was captured after a timeout of {(dt - lastCall).TotalMilliseconds:F0} ms"
                            : $"Locker {locker.Id} was captured first time");
                    callStats[locker.Id] = dt;
                    locker.UnlockAfterMs(5000);
                }).Start();
            }

            
            Console.ReadKey();
            syncPool.Dispose();
        }

    }
}
