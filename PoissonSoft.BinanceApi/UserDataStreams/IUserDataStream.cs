using System;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;

namespace PoissonSoft.BinanceApi.UserDataStreams
{
    /// <summary>
    /// User Data Stream
    /// </summary>
    public interface IUserDataStream
    {
        /// <summary>
        /// Тип User Data Stream 
        /// </summary>
        UserDataStreamType StreamType { get; }

        /// <summary>
        /// Инструмент, данные по которому приходят из этого потока
        /// (актуально только для типа IsolatedMargin)
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// Текущее состояние потока
        /// </summary>
        UserDataStreamStatus Status { get; }

        /// <summary>
        /// Событие обновления балансов аккаунта
        /// </summary>
        event EventHandler<AccountUpdatePayload> OnAccountUpdate;

        /// <summary>
        /// Событие обновления балансов аккаунта в результате ввода/вывода или внутреннего перевода
        /// между счетами
        /// </summary>
        event EventHandler<BalanceUpdatePayload> OnBalanceUpdate;

        /// <summary>
        /// Событие изменения ордера
        /// </summary>
        event EventHandler<OrderExecutionReportPayload> OnOrderExecuteEvent;

        /// <summary>
        /// Дополнительная информация при изменении OCO-ордера
        /// https://academy.binance.com/ru/articles/what-is-an-oco-order
        /// </summary>
        event EventHandler<OrderListStatusPayload> OnOrderListStatusEvent;

        /// <summary>
        /// Открытие (запуск) потока
        /// </summary>
        void Open();

        /// <summary>
        /// Закрытие потока
        /// </summary>
        void Close();
    }

    /// <summary>
    /// Тип User Data Stream
    /// </summary>
    public enum UserDataStreamType
    {
        /// <summary>
        /// Неизвестный тип (ошибка)
        /// </summary>
        Unknown, 

        /// <summary>
        /// Спотовый аккаунт
        /// </summary>
        Spot,

        /// <summary>
        /// Маржинальный аккаунт
        /// </summary>
        Margin,

        /// <summary>
        /// Маржинальный аккаунт для отдельного инструмента
        /// </summary>
        IsolatedMargin
    }

    /// <summary>
    /// Текущее состояние User Data Stream
    /// </summary>
    public enum UserDataStreamStatus
    {
        /// <summary>
        /// Неизвестное состояние (ошибка)
        /// </summary>
        Unknown,

        /// <summary>
        /// Поток активен
        /// </summary>
        Active,

        /// <summary>
        /// Поток закрыт
        /// </summary>
        Closed,

    }
}
