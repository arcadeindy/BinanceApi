using System;
using System.Linq;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Filters;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.Contracts
{
    /// <summary>
    /// Торговый инструмент
    /// </summary>
    public class TradeInstrument: ICloneable
    {
        /// <summary>
        /// Тикер
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumExConverter), InstrumentStatus.Unknown)]
        public InstrumentStatus Status { get; set; }

        /// <summary>
        /// Базовая монета
        /// </summary>
        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; }

        /// <summary>
        /// Точность котировки базовой монеты
        /// </summary>
        [JsonProperty("baseAssetPrecision")]
        public int BaseAssetPrecision { get; set; }

        /// <summary>
        /// Котируемая монета
        /// </summary>
        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; }

        /// <summary>
        /// Точность котировки котируемой монеты
        /// </summary>
        [JsonProperty("quoteAssetPrecision")]
        public int QuoteAssetPrecision { get; set; }

        /// <summary>
        /// Точность котировки инструмента
        /// </summary>
        [JsonProperty("quotePrecision")]
        public int QuotePrecision { get; set; }

        /// <summary>
        /// "baseCommissionPrecision": 8,
        /// </summary>
        [JsonProperty("baseCommissionPrecision")]
        public int BaseCommissionPrecision { get; set; }

        /// <summary>
        /// "quoteCommissionPrecision": 8,
        /// </summary>
        [JsonProperty("quoteCommissionPrecision")]
        public int QuoteCommissionPrecision { get; set; }
        
        /// <summary>
        /// Допустимые типы ордеров
        /// </summary>
        [JsonProperty("orderTypes", 
            ItemConverterType = typeof(StringEnumExConverter), 
            ItemConverterParameters = new object[]{OrderType.Unknown})]
        public OrderType[] OrderTypes { get; set; }

        /// <summary>
        /// "icebergAllowed": true,
        /// </summary>
        [JsonProperty("icebergAllowed")]
        public bool IcebergAllowed { get; set; }

        /// <summary>
        /// "ocoAllowed": true,
        /// </summary>
        [JsonProperty("ocoAllowed")]
        public bool OcoAllowed { get; set; }

        /// <summary>
        /// "quoteOrderQtyMarketAllowed": true,
        /// </summary>
        [JsonProperty("quoteOrderQtyMarketAllowed")]
        public bool QuoteOrderQtyMarketAllowed { get; set; }

        /// <summary>
        /// "isSpotTradingAllowed": true,
        /// </summary>
        [JsonProperty("isSpotTradingAllowed")]
        public bool IsSpotTradingAllowed { get; set; }

        /// <summary>
        /// "isMarginTradingAllowed": true,
        /// </summary>
        [JsonProperty("isMarginTradingAllowed")]
        public bool IsMarginTradingAllowed { get; set; }
        
        /// <summary>
        /// Фильтры
        /// </summary>
        [JsonProperty(PropertyName = "filters", ItemConverterType = typeof(FilterConverter))]
        public SymbolFilter[] Filters { get; set; }

        /// <summary>
        /// На каких рынках доступен это инструмент
        ///  "permissions": ["SPOT", "MARGIN"]
        /// </summary>
        [JsonProperty("permissions",
            ItemConverterType = typeof(StringEnumExConverter),
            ItemConverterParameters = new object[] {TradeSectionType.Unknown})]
        public TradeSectionType[] Permissions { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new TradeInstrument
            {
                Symbol = Symbol,
                Status = Status,
                BaseAsset = BaseAsset,
                BaseAssetPrecision = BaseAssetPrecision,
                QuoteAsset = QuoteAsset,
                QuoteAssetPrecision = QuoteAssetPrecision,
                QuotePrecision = QuotePrecision,
                BaseCommissionPrecision = BaseCommissionPrecision,
                QuoteCommissionPrecision = QuoteCommissionPrecision,
                OrderTypes = OrderTypes?.ToArray(),
                IcebergAllowed = IcebergAllowed,
                OcoAllowed = OcoAllowed,
                QuoteOrderQtyMarketAllowed = QuoteOrderQtyMarketAllowed,
                IsSpotTradingAllowed = IsSpotTradingAllowed,
                IsMarginTradingAllowed = IsMarginTradingAllowed,
                Filters = Filters?.Select(x => (SymbolFilter)x.Clone()).ToArray(),
                Permissions = Permissions?.ToArray()
            };
        }
    }
}
