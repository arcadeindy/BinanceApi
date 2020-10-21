using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.CommonUtils.ConsoleUtils;

namespace BinanceApi.Example
{
    internal partial class ActionManager
    {
        private bool ShowSpotAccountApiPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.A] = "Account Information",

                [ConsoleKey.Escape] = "Go back",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.A:
                    SafeCall(() =>
                    {
                        var exchangeInfo = apiClient.SpotAccountApi.GetAccountInformation();
                        Console.WriteLine(JsonConvert.SerializeObject(exchangeInfo, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private bool ShowSpotDataStreamPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.Escape] = "Go back",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        private bool ShowSpotDataCollectorPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.B] = "Start SpotDataCollector",
                [ConsoleKey.E] = "Stop SpotDataCollector",
                [ConsoleKey.Escape] = "Go back",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.B:
                    SafeCall(() =>
                    {
                        apiClient.SpotDataCollector.Start();
                        Console.WriteLine($"SpotDataCollector.IsStarted={apiClient.SpotDataCollector.IsStarted}");
                    });
                    return true;
                case ConsoleKey.E:
                    SafeCall(() =>
                    {
                        apiClient.SpotDataCollector.Stop();
                        Console.WriteLine($"SpotDataCollector.IsStarted={apiClient.SpotDataCollector.IsStarted}");
                    });
                    return true;
                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }
    }
}
