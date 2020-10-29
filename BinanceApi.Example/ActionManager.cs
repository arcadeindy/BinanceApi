using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi;
using PoissonSoft.CommonUtils.ConsoleUtils;

namespace BinanceApi.Example
{
    internal partial class ActionManager
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
                [ConsoleKey.A] = "Wallet API",

                [ConsoleKey.B] = "Market Data API",
                [ConsoleKey.C] = "Market Data Streams",

                [ConsoleKey.D] = "Spot Account API",
                [ConsoleKey.E] = "Spot Data Stream",
                [ConsoleKey.F] = "Spot Data Collector",

                [ConsoleKey.Escape] = "Go back (exit)",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.A:
                    // TODO:
                    Console.WriteLine("Not implemented yet");
                    return true;

                case ConsoleKey.B:
                    while (ShowMarketDataPage()) { }
                    return true;

                case ConsoleKey.C:
                    while (ShowMarketDataStreamsPage()) { }
                    return true;

                case ConsoleKey.D:
                    while (ShowSpotAccountApiPage()) { }
                    return true;

                case ConsoleKey.E:
                    while (ShowSpotDataStreamPage()) { }
                    return true;

                case ConsoleKey.F:
                    while (ShowSpotDataCollectorPage()) { }
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
