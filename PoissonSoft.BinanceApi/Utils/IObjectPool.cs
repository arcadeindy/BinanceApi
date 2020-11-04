namespace PoissonSoft.BinanceApi.Utils
{
    /// <summary>
    /// Пул произвольных объектов
    /// </summary>
    /// <typeparam name="TEntity">Тип объекта в пуле</typeparam>
    public interface IObjectPool<TEntity>
    {
        /// <summary>
        /// Исходный размер пула
        /// </summary>
        int StartSize { get; set; }

        /// <summary>
        /// Минимальный размер пула.
        /// Пул должен поддерживать число свободных объектов не меньше этого значения
        /// </summary>
        int MinSize { get; set; }

        /// <summary>
        /// Максимальный размер пула
        /// </summary>
        int MaxSize { get; set; }

        /// <summary>
        /// Шаг изменения размера пула
        /// </summary>
        int SizeIncrement { get; set; }
        
        /// <summary>
        /// Максимальное количество доступных объектов
        /// Если количество доступных объектов больше этого значения, пул должен уничтожить лишние объекты
        /// </summary>
        int AvailableLimit { get; set; }



        /// <summary>
        /// Текущий размер пула - число объектов, содержащихся в пуле
        /// </summary>
        int CurrentSize { get; }

        /// <summary>
        /// Количество свободных (не используемых потребителями пула) объектов
        /// </summary>
        int AvailableObjectsCount { get; }

        /// <summary>
        /// Реально достигнутый максимум элементов в пуле за время его работы
        /// </summary>
        int AchievedMaximum { get; }

        /// <summary>
        /// Реально достигнутый минимум элементов в пуле за время его работы
        /// </summary>
        int AchievedMinimum { get; }

        /// <summary>
        /// Получение объекта из пула
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool TryGetEntity(out TEntity entity);

        /// <summary>
        /// Возврат использованного объекта в пул
        /// </summary>
        /// <param name="entity"></param>
        void ReturnToPool(TEntity entity);
    }
}
