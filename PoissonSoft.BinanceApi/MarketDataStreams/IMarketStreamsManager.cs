using System;
using System.Collections.Generic;
using System.Text;
using PoissonSoft.BinanceApi.Contracts.MarketDataStream;

namespace PoissonSoft.BinanceApi.MarketDataStreams
{
    /// <summary>
    /// Manager for Websocket Market Streams
    /// </summary>
    public interface IMarketStreamsManager
    {
        /// <summary>
        /// Подписка на верхние <paramref name="levelsCount"/> уровней стакана
        /// </summary>
        /// <param name="symbol">Тикер торгового инструмента</param>
        /// <param name="levelsCount">Количество уровней ask/bid в ответе. Допустимые значения 5, 10, 20.
        /// Переданное значение будет округлено до ближайшего допустимого</param>
        /// <param name="updateSpeedMs">Интервал между поступление обновлений в миллисекундах. Допустимые значения 1000, 100.
        /// Переданное значение будет округлено до ближайшего допустимого</param>
        /// <param name="callbackAction">Функция обратного вызова. Будет вызываться каждый раз при получении обновления</param>
        /// <returns></returns>
        SubscriptionInfo SubscribePartialBookDepth(string symbol, int levelsCount, int updateSpeedMs, Action<PartialBookDepthPayload> callbackAction);


        /// <summary>
        /// Unsubscribe
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        bool Unsubscribe(long subscriptionId);

    }
}
