using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.MarketData;
using PoissonSoft.BinanceApi.Contracts.MarketDataStream;
using PoissonSoft.CommonUtils.ConsoleUtils;

namespace BinanceApi.Example
{
    internal partial class ActionManager
    {
        private bool ShowMarketDataPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.I] = "Exchange Information",
                [ConsoleKey.B] = "Order book",
                [ConsoleKey.Escape] = "Go back (to main menu)",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.I:
                    SafeCall(() =>
                    {
                        var exchangeInfo = apiClient.MarketDataApi.GetExchangeInfo();
                        Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.B:
                    SafeCall(() =>
                    {
                        var req = new OrderBookRequest
                        {
                            Symbol = InputHelper.GetString("Symbol: "),
                            Limit = InputHelper.GetInt("Limit: ")
                        };
                        var exchangeInfo = apiClient.MarketDataApi.OrderBook(req);
                        Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented));
                    });
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private bool ShowMarketDataStreamsPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.P] = "Subscribe Partial Book Depth",

                [ConsoleKey.U] = "Unsubscribe by subscription Id",
                [ConsoleKey.V] = "Unsubscribe all subscriptions",

                [ConsoleKey.Escape] = "Go back (to main menu)",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.P:
                    SubscribePartialBookDepth();
                    return true;

                case ConsoleKey.U:
                    SafeCall(() =>
                    {
                        var subscriptionId = InputHelper.GetLong("Subscription id to unsubscribe:");
                        var result = apiClient.MarketStreamsManager.Unsubscribe(subscriptionId);
                        Console.WriteLine($"Unsubscribe result = {result}");
                    });
                    return true;

                case ConsoleKey.V:
                    SafeCall(() =>
                    {
                        apiClient.MarketStreamsManager.UnsubscribeAll();
                    });
                    return true;

                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private void SubscribePartialBookDepth()
        {
            var symbol = InputHelper.GetString("Symbol:");
            var levelsCount = InputHelper.GetInt("Levels count (5, 10, 20):");
            var updateSpeed = InputHelper.GetInt("Update speed in ms (100 or 1000):");

            try
            {
                var subscriptionInfo = apiClient.MarketStreamsManager.SubscribePartialBookDepth(symbol, levelsCount,
                    updateSpeed, OnPayloadReceived);

                Console.WriteLine($"Subscription Info:\n{JsonConvert.SerializeObject(subscriptionInfo, Formatting.Indented)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnPayloadReceived<TPayload>(TPayload payload)
        {
            Console.WriteLine(JsonConvert.SerializeObject(payload, Formatting.Indented));
        }
    }
}
