using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoissonSoft.BinanceApi.Contracts;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.Transport;
using PoissonSoft.BinanceApi.UserDataStreams;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    internal sealed class SpotDataCollector: ISpotDataCollector, IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly object startLock = new object();
        private readonly string userFriendlyName = nameof(SpotDataCollector);

        private AccountInformation accountInformation;
        private long lastUpdateTimestamp;
        private readonly ConcurrentQueue<AccountUpdatePayload> accountUpdatesQueue = new ConcurrentQueue<AccountUpdatePayload>();
        private CancellationTokenSource cancellationTokenSource;
        private Thread queueDispatchingThread;


        public SpotDataCollector(BinanceApiClient apiClient)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public void Start()
        {
            lock (startLock)
            {
                if (IsStarted) return;

                while (true)
                {
                    try
                    {
                        TryStart();
                        break;
                    }
                    catch (Exception e)
                    {
                        apiClient.Logger.Error($"{userFriendlyName}. Exception when starting Collector\n{e}");
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }

                IsStarted = true;
            }
        }

        public void Stop()
        {
            lock (startLock)
            {
                if (!IsStarted) return;

                while (true)
                {
                    try
                    {
                        TryStop();
                        break;
                    }
                    catch (Exception e)
                    {
                        apiClient.Logger.Error($"{userFriendlyName}. Exception when stopping Collector\n{e}");
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }

                IsStarted = false;
            }
        }

        public bool IsStarted { get; private set; }

        public AccountInformation AccountInformation
        {
            get => (AccountInformation) accountInformation?.Clone();
            private set => Interlocked.Exchange(ref accountInformation, value);
        }

        private void TryStart()
        {
            var timeout = TimeSpan.Zero;

            apiClient.SpotDataStream.OnAccountUpdate += OnAccountUpdate;

            // Включение SpotDataStream
            var nextProblemInform = DateTimeOffset.UtcNow.AddMinutes(1);
            while (true)
            {
                if (apiClient.SpotDataStream.Status == DataStreamStatus.Active) break;

                if (apiClient.SpotDataStream.Status == DataStreamStatus.Closed)
                {
                    apiClient.SpotDataStream.Open();
                    timeout = TimeSpan.FromSeconds(1);
                }
                else if (timeout.TotalSeconds < 15)
                {
                    timeout += TimeSpan.FromSeconds(1);
                }

                Thread.Sleep(timeout);

                if (DateTimeOffset.UtcNow > nextProblemInform)
                {
                    apiClient.Logger.Warn($"{userFriendlyName}. Проблема инициализации: Не удаётся включить {nameof(apiClient.SpotDataStream)}");
                    nextProblemInform = DateTimeOffset.UtcNow.AddMinutes(1);
                }
            }
            apiClient.Logger.Info($"{userFriendlyName}. Инициализация: {nameof(apiClient.SpotDataStream)} успешно включен");

            // Загрузка стартового снапшота AccountInformation
            timeout = TimeSpan.Zero;
            nextProblemInform = DateTimeOffset.UtcNow.AddMinutes(1);
            while (true)
            {
                AccountInformation snapshot = null;
                try
                {
                    snapshot = apiClient.SpotAccountApi.GetAccountInformation();
                }
                catch (Exception e)
                {
                    apiClient.Logger.Error($"{userFriendlyName}. Проблема инициализации: Исключение при попытке загрузить стартовый снапшот {nameof(AccountInformation)}\n{e}");
                }

                if (snapshot != null)
                {
                    lastUpdateTimestamp = snapshot.UpdateTimestamp;
                    AccountInformation = snapshot;
                    break;
                }

                if (timeout.TotalSeconds < 15)
                {
                    timeout += TimeSpan.FromSeconds(1);
                }

                Thread.Sleep(timeout);

                if (DateTimeOffset.UtcNow > nextProblemInform)
                {
                    apiClient.Logger.Warn($"{userFriendlyName}. Проблема инициализации: Не удаётся загрузить стартовый снапшот {nameof(AccountInformation)}");
                    nextProblemInform = DateTimeOffset.UtcNow.AddMinutes(1);
                }
            }

            cancellationTokenSource = new CancellationTokenSource();
            queueDispatchingThread = new Thread(QueueDispatching);
            queueDispatchingThread.Start(cancellationTokenSource.Token);
        }

        private void TryStop()
        {
            apiClient.SpotDataStream.OnAccountUpdate -= OnAccountUpdate;
            cancellationTokenSource?.Cancel();
            queueDispatchingThread?.Join(TimeSpan.FromSeconds(30));
            AccountInformation = null;
        }

        private void OnAccountUpdate(object sender, AccountUpdatePayload update)
        {
            accountUpdatesQueue.Enqueue(update);
        }

        private void QueueDispatching(object ct)
        {
            var cancellationToken = (CancellationToken) ct;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (!accountUpdatesQueue.IsEmpty)
                {
                    try
                    {
                        ProcessUpdates();
                    }
                    catch (Exception e)
                    {
                        apiClient.Logger.Error($"{userFriendlyName}. Exception when processing Account Updates\n{e}");
                    }
                }
                
                Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task => { })
                    // ReSharper disable once MethodSupportsCancellation
                    .Wait();
            }
        }

        private void ProcessUpdates()
        {
            var updates = new List<AccountUpdatePayload>(accountUpdatesQueue.Count);
            while (accountUpdatesQueue.TryDequeue(out var update))
            {
                if (update.LastAccountUpdateTime > lastUpdateTimestamp)
                {
                    updates.Add(update);
                }
            }

            if (!updates.Any()) return;

            var snapshot = AccountInformation;
            if (snapshot == null) return;

            foreach (var update in updates)
            {
                if (update.ChangedBalances?.Any() != true) continue;

                foreach (var actualBalance in update.ChangedBalances)
                {
                    var balanceToEdit = snapshot.Balances.FirstOrDefault(x => x.Asset == actualBalance.Asset);
                    if (balanceToEdit == null)
                    {
                        balanceToEdit = new Balance
                        {
                            Asset = actualBalance.Asset
                        };
                        snapshot.Balances.Add(balanceToEdit);
                    }

                    balanceToEdit.Free = actualBalance.Free;
                    balanceToEdit.Locked = actualBalance.Locked;
                }

                snapshot.UpdateTimestamp = update.LastAccountUpdateTime;
            }

            AccountInformation = snapshot;
        }


        public void Dispose()
        {
            if (IsStarted) Stop();

            apiClient?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }
}
