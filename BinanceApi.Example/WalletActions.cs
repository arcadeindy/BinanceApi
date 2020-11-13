using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.SpotAccount;
using PoissonSoft.BinanceApi.Contracts.UserDataStream;
using PoissonSoft.BinanceApi.Contracts.Wallet;
using PoissonSoft.BinanceApi.UserDataStreams;
using PoissonSoft.CommonUtils.ConsoleUtils;

namespace BinanceApi.Example
{
    internal partial class ActionManager
    {
        private bool ShowWalletApiPage()
        {
            var actions = new Dictionary<ConsoleKey, string>()
            {
                [ConsoleKey.A] = "System Status",

                [ConsoleKey.B] = "All Coins' Information",
                [ConsoleKey.C] = "Daily Account Snapshot",

                [ConsoleKey.D] = "Disable Fast Withdraw Switch",
                [ConsoleKey.E] = "Enable Fast Withdraw Switch",

                [ConsoleKey.F] = "Withdraw [SAPI]",
                //[ConsoleKey.G] = "Withdraw [WAPI]",

                [ConsoleKey.H] = "Deposit History (supporting network)",
                //[ConsoleKey.I] = "Deposit History",
                [ConsoleKey.J] = "Withdraw History (supporting network)",
                //[ConsoleKey.K] = "Withdraw History",
                [ConsoleKey.L] = "Deposit Address (supporting network)",
                //[ConsoleKey.M] = "Deposit Address",

                [ConsoleKey.N] = "Account Status",
                [ConsoleKey.O] = "Account API Trading Status",

                [ConsoleKey.P] = "DustLog",
                [ConsoleKey.Q] = "Dust Transfer",
                [ConsoleKey.R] = "Asset Dividend Record",
                [ConsoleKey.S] = "Asset Detail",
                [ConsoleKey.T] = "Trade Fee",
      
                [ConsoleKey.Escape] = "Go back",
            };

            var selectedAction = InputHelper.GetUserAction("Select action:", actions);


            switch (selectedAction)
            {
                case ConsoleKey.B: // All Coins' Information
                    SafeCall(() =>
                    {
                        var data = apiClient.WalletApi.AllCoinsInformation();
                        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.F: // Withdraw [SAPI]
                    SafeCall(() =>
                    {
                        var withdrawRequest = new WithdrawRequest
                        {
                            Coin = InputHelper.GetString("Coin: "),
                            Address = InputHelper.GetString("Address: "),
                            AddressTag = InputHelper.GetString("AddressTag: "),
                            Amount = InputHelper.GetDecimal("Amount: ")
                        };
                        if (withdrawRequest.AddressTag == string.Empty) withdrawRequest.AddressTag = null;

                        if (!InputHelper.Confirm($"Warning! Do you really want to withdraw {withdrawRequest.Amount} {withdrawRequest.Coin} " +
                                                 $"to address {withdrawRequest.Address}|{withdrawRequest.AddressTag}?")) return;

                        var data = apiClient.WalletApi.Withdraw(withdrawRequest);
                        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.H: // Deposit History (supporting network)
                    SafeCall(() =>
                    {
                        var data = apiClient.WalletApi.DepositHistory(new DepositHistoryRequest
                            {
                                Coin = InputHelper.GetString("Coin: "),
                            });
                        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.J: // Withdraw History (supporting network)
                    SafeCall(() =>
                    {
                        var data = apiClient.WalletApi.WithdrawHistory(new WithdrawHistoryRequest
                        {
                            Coin = InputHelper.GetString("Coin: "),
                        });
                        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.L: // Deposit Address (supporting network)
                    SafeCall(() =>
                    {
                        var data = apiClient.WalletApi.DepositAddress(
                            InputHelper.GetString("Coin to deposit: "));
                        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                    });
                    return true;

                case ConsoleKey.Escape:
                    return false;

                default:
                    if (actions.ContainsKey(selectedAction))
                    {
                        Console.WriteLine($"Method '{actions[selectedAction]}' is not implemented");
                    }
                    return true;
            }
        }

    }
}
