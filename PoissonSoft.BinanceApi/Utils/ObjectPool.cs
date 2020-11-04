using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PoissonSoft.BinanceApi.Utils
{
    /// <summary>
    /// Базовый абстрактный класс пула
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ObjectPool<TEntity>: IObjectPool<TEntity>, IDisposable
    {
        private bool isDisposed;

        private readonly ConcurrentBag<TEntity> availableObjects = new ConcurrentBag<TEntity>();
        private int objectsInUse;
        private readonly bool checksReturnedObjectsForNull;

        private readonly Task poolManagementTask;
        private readonly CancellationTokenSource stoppingTokenSource;
        private readonly CancellationToken stoppingToken;

        /// <inheritdoc />
        public int StartSize { get; set; }

        /// <inheritdoc />
        public int MinSize { get; set; }

        /// <inheritdoc />
        public int MaxSize { get; set; }

        /// <inheritdoc />
        public int SizeIncrement { get; set; }

        /// <inheritdoc />
        public int AvailableLimit { get; set; }

        /// <inheritdoc />
        public int CurrentSize => availableObjects.Count + objectsInUse;

        /// <summary>
        /// Количество используемых объектов
        /// </summary>
        public int ObjectsInUse => objectsInUse;

        /// <inheritdoc />
        public int AvailableObjectsCount => availableObjects.Count;

        /// <inheritdoc />
        public int AchievedMaximum { get; private set; }

        /// <inheritdoc />
        public int AchievedMinimum { get; private set; }

        /// <summary>
        /// Частота выполнения процедуры управления пулом
        /// </summary>
        protected TimeSpan PollManagementTimeout { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        protected ObjectPool(int startSize = 10, int minSize = 3, int maxSize = int.MaxValue, 
            int sizeIncrement = 1, int availableLimit = 100)
        
        {
            if (startSize < minSize)
                throw new ArgumentException($"{nameof(startSize)}={startSize} must be greater " +
                                            $"than {nameof(minSize)}={minSize}");

            if (minSize < 1)
                throw new ArgumentException($"{nameof(minSize)}={minSize} must be greater than 0");
            
            if (sizeIncrement > minSize)
                throw new ArgumentException($"{nameof(sizeIncrement)}={sizeIncrement} must be less or equals " +
                                            $"than {nameof(minSize)}={minSize}");

            if (sizeIncrement > maxSize - minSize)
                throw new ArgumentException($"{nameof(sizeIncrement)}={sizeIncrement} must be less or equals " +
                                            $"than the range of size ({nameof(maxSize)}-{nameof(minSize)}={maxSize-minSize}");
            if (availableLimit <= minSize)
                throw new ArgumentException($"{nameof(availableLimit)}={availableLimit} must be greater " +
                                        $"than {nameof(minSize)}={minSize}");

            StartSize = startSize;
            MinSize = minSize;
            MaxSize = maxSize;
            SizeIncrement = sizeIncrement;
            AvailableLimit = availableLimit;
            
            checksReturnedObjectsForNull = typeof(TEntity).IsClass;

            IncreaseSize(StartSize);
            AchievedMinimum = CurrentSize;

            PollManagementTimeout = TimeSpan.FromSeconds(1);

            stoppingTokenSource = new CancellationTokenSource();
            stoppingToken = stoppingTokenSource.Token;
            poolManagementTask = Task.Run(PollManagementThread);
        }

        /// <inheritdoc />
        public bool TryGetEntity(out TEntity entity)
        {
            if (isDisposed)
            {
                entity = default;
                return false;
            }

            var success = availableObjects.TryTake(out entity);
            if (success)
            {
                Interlocked.Increment(ref objectsInUse);
                return true;
            }

            return CurrentSize < MaxSize && TryIncreaseSizeAndGetEntity(SizeIncrement, out entity);
        }

        /// <inheritdoc />
        public void ReturnToPool(TEntity entity)
        {
            if (isDisposed) return;
            
            if (checksReturnedObjectsForNull && EqualityComparer<TEntity>.Default.Equals(entity, default)) return;

            if (!availableObjects.Contains(entity))
            {
                Interlocked.Decrement(ref objectsInUse);
                availableObjects.Add(entity);
            }
        }

        /// <summary>
        /// Создание экземпляра объекта
        /// Метод должен быть определён в классах-наследниках
        /// </summary>
        /// <returns></returns>
        protected abstract TEntity CreateEntity();

        /// <summary>
        /// Освобождение ресурсов объекта
        /// По умолчанию для IDisposable-объектов вызывается метод Dispose
        /// Это поведение может быть переопределено в классах-наследниках
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void DisposeEntity(TEntity entity)
        {
            (entity as IDisposable)?.Dispose();
        }

        private void IncreaseSize(int cnt)
        {
            var addedCnt = 0;
            while (addedCnt < cnt && CurrentSize < MaxSize )
            {
                var entity = CreateEntity();
                availableObjects.Add(entity);
                addedCnt++;
            }

            if (CurrentSize > AchievedMaximum) AchievedMaximum = CurrentSize;
        }

        private bool TryIncreaseSizeAndGetEntity(int cnt, out TEntity entity)
        {
            if (CurrentSize < MaxSize)
            {
                entity = CreateEntity();
                Interlocked.Increment(ref objectsInUse);
                if (CurrentSize > AchievedMaximum) AchievedMaximum = CurrentSize;
                if (cnt > 1) IncreaseSize(cnt - 1);
                return true;
            }

            entity = default;
            return false;
        }

        private void DecreaseSize(int cnt)
        {
            var removedCnt = 0;
            while (removedCnt < cnt && AvailableObjectsCount > 0)
            {
                if (availableObjects.TryTake(out var obj))
                {
                    DisposeEntity(obj);
                    removedCnt++;
                }
                else
                {
                    break;
                }
            }

            if (CurrentSize < AchievedMinimum) AchievedMinimum = CurrentSize;
        }

        /// <summary>
        /// "Ручной" запуск процедуры менеджмента пула. В нормальном состоянии этот метод вызывается по таймеру
        /// </summary>
        public void ForcePollManagement()
        {
            try
            {
                if (AvailableObjectsCount < MinSize) IncreaseSize(SizeIncrement);
                else if (AvailableObjectsCount > AvailableLimit) DecreaseSize(SizeIncrement);
            }
            catch
            {
                // ignore
            }
        }

        private void PollManagementThread()
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Task.Delay(PollManagementTimeout, stoppingToken).Wait(stoppingToken);
                ForcePollManagement();
            }
        }

        private void DisposeCollection(ConcurrentBag<TEntity> collection)
        {
            try
            {
                while (!collection.IsEmpty)
                {
                    if (collection.TryTake(out var obj))
                    {
                        DisposeEntity(obj);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true;
                stoppingTokenSource.Cancel();

                try
                {
                    if (poolManagementTask.Status != TaskStatus.Canceled)
                    {
                        poolManagementTask.Wait(TimeSpan.FromSeconds(30));
                    }
                }
                catch
                {
                    // ignore
                }

                DisposeCollection(availableObjects);

                poolManagementTask?.Dispose();
                stoppingTokenSource?.Dispose();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
