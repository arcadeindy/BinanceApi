using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.UserDataStreams;
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
                [ConsoleKey.O] = "Open stream",
                [ConsoleKey.C] = "Close stream",

                [ConsoleKey.Escape] = "Go back",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);

            switch (selectedAction)
            {
                case ConsoleKey.O:
                    OpenSpotUserDataStream();
                    return true;

                case ConsoleKey.C:
                    CloseSpotUserDataStream();
                    return true;

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

                [ConsoleKey.A] = "Account Information",

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

                case ConsoleKey.A:
                    SafeCall(() =>
                    {
                        var ai = apiClient.SpotDataCollector.AccountInformation;
                        Console.WriteLine(JsonConvert.SerializeObject(ai, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.Escape:
                    return false;
                default:
                    return true;
            }
        }

        #region [User Data Stream]
        private void OpenSpotUserDataStream()
        {
            try
            {
                apiClient.SpotDataStream.OnAccountUpdate += SpotDataStreamOnAccountUpdate;
                apiClient.SpotDataStream.OnBalanceUpdate += SpotDataStreamOnBalanceUpdate;
                apiClient.SpotDataStream.OnOrderExecuteEvent += SpotDataStreamOnOrderExecuteEvent;
                apiClient.SpotDataStream.OnOrderListStatusEvent += SpotDataStreamOnOrderListStatusEvent;
                apiClient.SpotDataStream.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CloseSpotUserDataStream()
        {
            try
            {
                apiClient.SpotDataStream.Close();
                apiClient.SpotDataStream.OnAccountUpdate -= SpotDataStreamOnAccountUpdate;
                apiClient.SpotDataStream.OnBalanceUpdate -= SpotDataStreamOnBalanceUpdate;
                apiClient.SpotDataStream.OnOrderExecuteEvent -= SpotDataStreamOnOrderExecuteEvent;
                apiClient.SpotDataStream.OnOrderListStatusEvent -= SpotDataStreamOnOrderListStatusEvent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SpotDataStreamOnAccountUpdate(object sender, AccountUpdatePayload e)
        {
            Console.WriteLine($"OnAccountUpdate event\n{JsonConvert.SerializeObject(e, Formatting.Indented)}");
        }

        private void SpotDataStreamOnBalanceUpdate(object sender, BalanceUpdatePayload e)
        {
            Console.WriteLine($"OnBalanceUpdate event\n{JsonConvert.SerializeObject(e, Formatting.Indented)}");
        }

        private void SpotDataStreamOnOrderExecuteEvent(object sender, OrderExecutionReportPayload e)
        {
            Console.WriteLine($"OnOrderExecuteEvent event\n{JsonConvert.SerializeObject(e, Formatting.Indented)}");
        }

        private void SpotDataStreamOnOrderListStatusEvent(object sender, OrderListStatusPayload e)
        {
            Console.WriteLine($"OnOrderListStatusEvent event\n{JsonConvert.SerializeObject(e, Formatting.Indented)}");
        }

        #endregion

    }
}
