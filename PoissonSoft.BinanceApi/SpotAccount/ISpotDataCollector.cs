using PoissonSoft.BinanceApi.Contracts;

namespace PoissonSoft.BinanceApi.SpotAccount
{
    /// <summary>
    /// Сборщик актуальных данных по Spot-аккаунту
    /// (балансы, ордеры, трейды)
    /// </summary>
    public interface ISpotDataCollector
    {
        /// <summary>
        /// Запуск сбора данных
        /// </summary>
        void Start();

        /// <summary>
        /// Остановка сбора данных
        /// </summary>
        void Stop();

        /// <summary>
        /// Сборщик запущен?
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Данные аккаунта
        /// </summary>
        AccountInformation AccountInformation { get; }
    }
}
