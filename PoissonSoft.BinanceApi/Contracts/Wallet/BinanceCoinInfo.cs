using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Coin Information
    /// </summary>
    public class BinanceCoinInfo: ICloneable
    {
        /// <summary>
        /// "coin": "BTC",
        /// </summary>
        [JsonProperty("coin")]
        public string CoinTicker { get; set; }

        /// <summary>
        /// "depositAllEnable": true,
        /// </summary>
        [JsonProperty("depositAllEnable")]
        public bool DepositAllEnable { get; set; }

        /// <summary>
        /// "free": "0.08074558",
        /// </summary>
        [JsonProperty("free")]
        public decimal FreeBalance { get; set; }

        /// <summary>
        /// "freeze": "0.00000000",
        /// </summary>
        [JsonProperty("freeze")]
        public decimal FreezeBalance { get; set; }

        /// <summary>
        /// "ipoable": "0.00000000",
        /// </summary>
        [JsonProperty("ipoable")]
        public decimal IpoableBalance { get; set; }

        /// <summary>
        /// "ipoing": "0.00000000",
        /// </summary>
        [JsonProperty("ipoing")]
        public decimal IpoingBalance { get; set; }

        /// <summary>
        /// "isLegalMoney": false,
        /// </summary>
        [JsonProperty("isLegalMoney")]
        public bool IsLegalMoney { get; set; }

        /// <summary>
        /// "locked": "0.00000000",
        /// </summary>
        [JsonProperty("locked")]
        public decimal LockedBalance { get; set; }

        /// <summary>
        /// "name": "Bitcoin",
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// "networkList": [...]
        /// </summary>
        [JsonProperty("networkList")]
        public BinanceCoinNetworkInfo[] Networks { get; set; }

        /// <summary>
        /// Default Network
        /// </summary>
        [JsonIgnore]
        public BinanceCoinNetworkInfo DefaultNetwork =>
            Networks?.FirstOrDefault(x => x.IsDefaultNetwork) ?? Networks?.FirstOrDefault();

        /// <summary>
        /// "storage": "0.00000000",
        /// </summary>
        [JsonProperty("storage")]
        public decimal StorageBalance { get; set; }

        /// <summary>
        /// "trading": true,
        /// </summary>
        [JsonProperty("trading")]
        public bool TradingAllowed { get; set; }

        /// <summary>
        /// "withdrawAllEnable": true,
        /// </summary>
        [JsonProperty("withdrawAllEnable")]
        public bool WithdrawAllEnable { get; set; }

        /// <summary>
        /// "withdrawing": "0.00000000"
        /// </summary>
        [JsonProperty("withdrawing")]
        public decimal WithdrawingBalance { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new BinanceCoinInfo
            {
                CoinTicker = CoinTicker,
                DepositAllEnable = DepositAllEnable,
                FreeBalance = FreeBalance,
                FreezeBalance = FreezeBalance,
                IpoableBalance = IpoableBalance,
                IpoingBalance = IpoingBalance,
                IsLegalMoney = IsLegalMoney,
                LockedBalance = LockedBalance,
                Name = Name,
                Networks = Networks?.Select(x => (BinanceCoinNetworkInfo)x.Clone()).ToArray(),
                StorageBalance = StorageBalance,
                TradingAllowed = TradingAllowed,
                WithdrawAllEnable = WithdrawAllEnable,
                WithdrawingBalance = WithdrawingBalance,
            };
        }
    }

    /// <summary>
    /// Block-chain network information
    /// </summary>
    public class BinanceCoinNetworkInfo: ICloneable
    {
        /// <summary>
        /// "addressRegex": "^(bnb1)[0-9a-z]{38}$",
        /// </summary>
        [JsonProperty("addressRegex")]
        public string AddressRegex { get; set; }

        /// <summary>
        /// "coin": "BTC",
        /// </summary>
        [JsonProperty("coin")]
        public string CoinTicker { get; set; }

        /// <summary>
        /// "depositDesc": "Wallet Maintenance, Deposit Suspended",
        /// Shown only when "depositEnable" is false.
        /// </summary>
        [JsonProperty("depositDesc")] // shown only when "depositEnable" is false.
        public string DepositDisabledReason { get; set; }

        /// <summary>
        /// "depositEnable": false,
        /// </summary>
        [JsonProperty("depositEnable")]
        public bool DepositEnable { get; set; }

        /// <summary>
        /// "isDefault": false,   
        /// </summary>
        [JsonProperty("isDefault")]
        public bool IsDefaultNetwork { get; set; }

        /// <summary>
        /// "memoRegex": "^[0-9A-Za-z\\-_]{1,120}$",
        /// </summary>
        [JsonProperty("memoRegex")]
        public string MemoRegex { get; set; }

        /// <summary>
        /// "minConfirm": 1,
        /// Min number for balance confirmation
        /// </summary>
        [JsonProperty("minConfirm")]
        public int MinConfirm { get; set; }

        /// <summary>
        /// "name": "BEP2",
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// "network": "BNB",
        /// </summary>
        [JsonProperty("network")]
        public string NetworkName { get; set; }
        
        /// <summary>
        /// "resetAddressStatus": false,
        /// </summary>
        [JsonProperty("resetAddressStatus")]
        public bool ResetAddressStatus { get; set; }

        /// <summary>
        /// "specialTips": "Both a MEMO and an Address are required to successfully deposit your BEP2-BTCB tokens to Binance.",
        /// </summary>
        [JsonProperty("specialTips")]
        public string SpecialTips { get; set; }
        
        /// <summary>
        /// "unLockConfirm": 0,
        /// Confirmation number for balance unlock 
        /// </summary>
        [JsonProperty("unLockConfirm")]
        public int UnLockConfirm { get; set; }

        /// <summary>
        /// "withdrawDesc": "Wallet Maintenance, Withdrawal Suspended",
        /// Shown only when "withdrawEnable" is false.
        /// </summary>
        [JsonProperty("withdrawDesc")] // shown only when "withdrawEnable" is false.
        public string WithdrawDisabledReason { get; set; }

        /// <summary>
        /// "withdrawEnable": false,
        /// </summary>
        [JsonProperty("withdrawEnable")]
        public bool WithdrawEnable { get; set; }

        /// <summary>
        /// "withdrawFee": "0.00000220",
        /// </summary>
        [JsonProperty("withdrawFee")]
        public decimal WithdrawFee { get; set; }

        /// <summary>
        /// "withdrawMin": "0.00000440"
        /// </summary>
        [JsonProperty("withdrawMin")]
        public decimal WithdrawMin { get; set; }

        /// <summary>
        /// "withdrawIntegerMultiple": "0.00000001",
        /// </summary>
        [JsonProperty("withdrawIntegerMultiple")]
        public decimal WithdrawStep { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new BinanceCoinNetworkInfo
            {
                AddressRegex = AddressRegex,
                CoinTicker = CoinTicker,
                DepositDisabledReason = DepositDisabledReason,
                DepositEnable = DepositEnable,
                IsDefaultNetwork = IsDefaultNetwork,
                MemoRegex = MemoRegex,
                MinConfirm = MinConfirm,
                Name = Name,
                NetworkName = NetworkName,
                ResetAddressStatus = ResetAddressStatus,
                SpecialTips = SpecialTips,
                UnLockConfirm = UnLockConfirm,
                WithdrawDisabledReason = WithdrawDisabledReason,
                WithdrawEnable = WithdrawEnable,
                WithdrawFee = WithdrawFee,
                WithdrawMin = WithdrawMin,
                WithdrawStep = WithdrawStep,
            };
        }
    }
}
