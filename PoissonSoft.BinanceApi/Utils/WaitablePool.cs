using System;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace PoissonSoft.BinanceApi.Utils
{
    internal sealed class WaitablePool: IDisposable
    {
        private readonly WaitHandle[] syncEvents;
        private readonly FeedLocker[] lockers;
        

        public WaitablePool(int feedsCount)
        {
            syncEvents = new WaitHandle[feedsCount];
            lockers = new FeedLocker[feedsCount];

            for (int i = 0; i < feedsCount; i++)
            {
                var evt = new AutoResetEvent(true);
                syncEvents[i] = evt;
                lockers[i] = new FeedLocker(evt, i);
            }
        }

        public FeedLocker Wait()
        {
            return lockers[WaitHandle.WaitAny(syncEvents)];
        }

        public void Dispose()
        {
            if (lockers?.Any() == true)
            {
                foreach (var locker in lockers) locker?.Dispose();
            }

            if (syncEvents?.Any() == true)
            {
                foreach (var syncEvent in syncEvents) syncEvent?.Dispose();
            }
        }
    }

    internal sealed class FeedLocker: IDisposable
    {
        private readonly AutoResetEvent syncEvent;
        private readonly Timer timer;

        public int Id { get; }

        public FeedLocker(AutoResetEvent syncEvent, int id)
        {
            this.syncEvent = syncEvent;
            Id = id;
            timer = new Timer {AutoReset = false, Enabled = false};
            timer.Elapsed += (sender, e) => syncEvent.Set();
        }

        public void UnlockAfterMs(int unlockTimeoutMs)
        {
            timer.Interval = unlockTimeoutMs;
            timer.Enabled = true;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
