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
            ThreadPool.SetMinThreads(100, 100);
            CheckUsingTask();
        }

        private static void CheckUsingThread()
        {
            var syncPool = new WaitablePool(5, 1);

            var callStats = new ConcurrentDictionary<int, DateTimeOffset>();

            for (int i = 0; i < 100; i++)
            {

                new Thread(id =>
                {
                    var highPriority = id != null && (int)id % 10 == 0;
                    var name = highPriority
                        ? $"High Priority thread #{id}"
                        : $"Standard thread #{id}";
                    Console.WriteLine($"{name} was started");

                    // ReSharper disable once AccessToDisposedClosure
                    var locker = syncPool.Wait(highPriority);
                    var dt = DateTimeOffset.UtcNow;
                    locker.UnlockAfterMs(5000);

                    Console.WriteLine(
                        callStats.TryGetValue(locker.Id, out var lastCall)
                            ? $"{name} finished. The locker {locker.Id} was used after a timeout of {(dt - lastCall).TotalMilliseconds:F0} ms"
                            : $"{name} finished. The locker {locker.Id} was used first time");
                    callStats[locker.Id] = dt;
                }).Start(i);

                Thread.Sleep(100);
            }

            Console.ReadKey();
            syncPool.Dispose();
        }

        private static void CheckUsingTask()
        {
            var syncPool = new WaitablePool(5, 1);

            var callStats = new ConcurrentDictionary<int, DateTimeOffset>();

            for (int i = 0; i < 100; i++)
            {
                var id = i;
                Task.Run(() =>
                {
                    var highPriority = id % 10 == 0;
                    var name = highPriority
                        ? $"High Priority thread #{id}"
                        : $"Standard thread #{id}";
                    Console.WriteLine($"{name} was started");

                    // ReSharper disable once AccessToDisposedClosure
                    var locker = syncPool.Wait(highPriority);
                    var dt = DateTimeOffset.UtcNow;
                    locker.UnlockAfterMs(5000);

                    Console.WriteLine(
                        callStats.TryGetValue(locker.Id, out var lastCall)
                            ? $"{name} finished. The locker {locker.Id} was used after a timeout of {(dt - lastCall).TotalMilliseconds:F0} ms"
                            : $"{name} finished. The locker {locker.Id} was used first time");
                    callStats[locker.Id] = dt;
                });

                Thread.Sleep(100);
            }

            Console.ReadKey();
            syncPool.Dispose();
        }

    }
}
