using System;

namespace PoissonSoft.BinanceApi.Utils
{
    /// <summary>
    /// Helper to check that some work has to be done or not at the moment
    /// </summary>
   public class SimpleScheduler
    {
        private int intervalMs;
        private DateTime nextRun;

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="initialState"></param>
        public SimpleScheduler(TimeSpan interval, bool initialState = true)
            : this((int)interval.TotalMilliseconds, initialState) { }

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        /// <param name="intervalMs"></param>
        /// <param name="initialState"></param>
        public SimpleScheduler(int intervalMs = 1000, bool initialState = true)
        {
            this.intervalMs = intervalMs;
            SetImmediateState(initialState);
        }

        /// <summary>
        /// Интервал планировщика
        /// </summary>
        public TimeSpan Interval
        {
            get => TimeSpan.FromMilliseconds(intervalMs);
            set => intervalMs = (int)value.TotalMilliseconds;
        }

        /// <summary>
        /// Проверка необходимости выполнить работу
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            return DateTime.Now >= nextRun;
        }

        /// <summary>
        /// Отметка об успешном выполнении работы
        /// </summary>
        public void Done()
        {
            nextRun = DateTime.Now.AddMilliseconds(intervalMs);
        }

        /// <summary>
        /// Ручная установка текущего состояния планировщика
        /// </summary>
        /// <param name="state"></param>
        public void SetImmediateState(bool state)
        {
            if (state) nextRun = DateTime.MinValue;
            else Done();
        }

    }
}