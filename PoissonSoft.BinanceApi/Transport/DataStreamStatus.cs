namespace PoissonSoft.BinanceApi.Transport
{
    /// <summary>
    /// Текущее состояние Data Stream
    /// </summary>
    public enum DataStreamStatus
    {
        /// <summary>
        /// Неизвестное состояние (ошибка)
        /// </summary>
        Unknown,

        /// <summary>
        /// Устанавливается соединение
        /// </summary>
        Connecting,

        /// <summary>
        /// Поток активен
        /// </summary>
        Active,

        /// <summary>
        /// Соединение восстанавливается после обрыва
        /// </summary>
        Reconnecting,

        /// <summary>
        /// Начат процесс остановки соединения
        /// </summary>
        Closing,

        /// <summary>
        /// Поток закрыт
        /// </summary>
        Closed,

    }
}