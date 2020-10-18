using System;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace PoissonSoft.BinanceApi.Utils
{
    internal sealed class WaitablePool: IDisposable
    {
        private readonly WaitHandle[] syncEventsStd;
        private readonly FeedLocker[] lockersStd;

        private readonly WaitHandle[] syncEventsHighPriority;
        private readonly FeedLocker[] lockersHighPriority;


        public WaitablePool(int feedsCount, int highPriorityFeedsCount)
        {
            syncEventsHighPriority = new WaitHandle[feedsCount];
            lockersHighPriority = new FeedLocker[feedsCount];

            syncEventsStd = new WaitHandle[feedsCount - highPriorityFeedsCount];
            lockersStd = new FeedLocker[feedsCount - highPriorityFeedsCount];

            for (int i = 0; i < feedsCount; i++)
            {
                var evt = new AutoResetEvent(true);
                syncEventsHighPriority[i] = evt;
                lockersHighPriority[i] = new FeedLocker(evt, i);

                if (i < feedsCount - highPriorityFeedsCount)
                {
                    syncEventsStd[i] = evt;
                    lockersStd[i] = lockersHighPriority[i];
                }
            }
        }

        public FeedLocker Wait(bool highPriority)
        {
            return highPriority 
                ? lockersHighPriority[WaitHandle.WaitAny(syncEventsHighPriority)]
                : lockersStd[WaitHandle.WaitAny(syncEventsStd)];
        }

        public void Dispose()
        {
            if (lockersStd?.Any() == true)
            {
                foreach (var locker in lockersStd) locker?.Dispose();
            }

            if (syncEventsStd?.Any() == true)
            {
                foreach (var syncEvent in syncEventsStd) syncEvent?.Dispose();
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
