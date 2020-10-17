using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi;
using PoissonSoft.CommonUtils.ConsoleUtils;

namespace BinanceApi.Example
{
    internal class ActionManager
    {
        private readonly BinanceApiClient apiClient;

        public ActionManager(BinanceApiClient apiClient)
        {
            this.apiClient = apiClient;
        }


        public void Run()
        {
            while (ShowMainPage()) { }
            Console.WriteLine("> The program stopped. Press any key to exit...");
            Console.ReadKey();
        }

        private bool ShowMainPage()
        {
            Console.Clear();
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.W] = "Wallet API",
                [ConsoleKey.M] = "Market Data API",
                [ConsoleKey.S] = "Spot Account/Trade API",
                [ConsoleKey.Escape] = "Go back (exit)",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.W:
                    // TODO:
                    Console.WriteLine("Not implemented yet");
                    return true;
                case ConsoleKey.M:
                    while (ShowMarketDataPage()) { }
                    return true;
                case ConsoleKey.S:
                    // TODO:
                    Console.WriteLine("Not implemented yet");
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private bool ShowMarketDataPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.I] = "Exchange Information",
                [ConsoleKey.Escape] = "Go back (to main menu)",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.I:
                    SafeCall(() =>
                    {
                        var exchangeInfo = apiClient.MarketDataApi.GetExchangeInfo();
                        Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented, new JsonSerializerSettings
                        {
                            MaxDepth = 1
                        }));
                    });
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private void SafeCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
