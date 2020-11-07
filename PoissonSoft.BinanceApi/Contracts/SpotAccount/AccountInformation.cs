using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Информация об аккаунте
    /// </summary>
    public class AccountInformation : ICloneable
    {
        /// <summary>
        /// Комиссия мейкера в bips (1 bips = 0.01%)
        /// "makerCommission": 15,
        /// </summary>
        [JsonProperty("makerCommission")]
        public decimal MakerCommission { get; set; }

        /// <summary>
        /// Комиссия тейкера в bips (1 bips = 0.01%)
        /// "takerCommission": 15,
        /// </summary>
        [JsonProperty("takerCommission")]
        public decimal TakerCommission { get; set; }

        /// <summary>
        /// "buyerCommission": 0,
        /// </summary>
        [JsonProperty("buyerCommission")]
        public int BuyerCommission { get; set; }

        /// <summary>
        /// "sellerCommission": 0,
        /// </summary>
        [JsonProperty("sellerCommission")]
        public int SellerCommission { get; set; }

        /// <summary>
        /// "canTrade": true,
        /// </summary>
        [JsonProperty("canTrade")]
        public bool CanTrade { get; set; }

        /// <summary>
        /// "canWithdraw": true,
        /// </summary>
        [JsonProperty("canWithdraw")]
        public bool CanWithdraw { get; set; }

        /// <summary>
        /// "canDeposit": true,
        /// </summary>
        [JsonProperty("canDeposit")]
        public bool CanDeposit { get; set; }

        /// <summary>
        /// "updateTime": 123456789,
        /// </summary>
        [JsonProperty("updateTime")]
        public long UpdateTimestamp { get; set; }

        /// <summary>
        /// "accountType": "SPOT",
        /// </summary>
        [JsonProperty("accountType")]
        [JsonConverter(typeof(StringEnumExConverter), TradeSectionType.Unknown)]
        public TradeSectionType AccountType { get; set; }

        /// <summary>
        /// Балансы
        /// </summary>
        [JsonProperty("balances")]
        public List<Balance> Balances { get; set; }

        /// <summary>
        ///   "permissions": ["SPOT"]
        /// </summary>
        [JsonProperty("permissions",
            ItemConverterType = typeof(StringEnumExConverter),
            ItemConverterParameters = new object[] { TradeSectionType.Unknown })]
        public TradeSectionType[] Permissions { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new AccountInformation
            {
                MakerCommission = MakerCommission,
                TakerCommission = TakerCommission,
                BuyerCommission = BuyerCommission,
                SellerCommission = SellerCommission,
                CanTrade = CanTrade,
                CanWithdraw = CanWithdraw,
                CanDeposit = CanDeposit,
                UpdateTimestamp = UpdateTimestamp,
                AccountType = AccountType,
                Balances = Balances?.Select(x => (Balance)x.Clone()).ToList(),
                Permissions = Permissions?.ToArray()
            };
        }
    }
}
