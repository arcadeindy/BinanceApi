using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PoissonSoft.BinanceApi.Contracts.Enums;
using PoissonSoft.BinanceApi.Contracts.Filters;
using PoissonSoft.BinanceApi.Contracts.Serialization;

namespace PoissonSoft.BinanceApi.UsdtFutures.Contracts
{
    /// <summary>
    /// Торговый инструмент (USDT-Futures)
    /// </summary>
    public class UFInstrument: ICloneable
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
        /// "maintMarginPercent": "2.5000",   // ignore
        /// </summary>
        [JsonProperty("maintMarginPercent")]
        public decimal MainMarginPercent { get; set; }

        /// <summary>
        /// "requiredMarginPercent": "5.0000",  // ignore
        /// </summary>
        [JsonProperty("requiredMarginPercent")]
        public decimal RequireMarginPercent { get; set; }
        
        /// <summary>
        /// Базовая монета
        /// </summary>
        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; }

        /// <summary>
        /// Котируемая монета
        /// </summary>
        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; }

        /// <summary>
        /// "marginAsset": "USDT",
        /// </summary>
        [JsonProperty("marginAsset")]
        public string MarginAsset { get; set; }

        /// <summary>
        /// Точность цены
        /// "pricePrecision": 5,
        /// </summary>
        [JsonProperty("pricePrecision")]
        public int PricePrecision { get; set; }

        /// <summary>
        /// Точность объёма
        /// "quantityPrecision": 0,
        /// </summary>
        [JsonProperty("quantityPrecision")]
        public int QuantityPrecision { get; set; }
        
        /// <summary>
        /// Точность котировки базовой монеты
        /// </summary>
        [JsonProperty("baseAssetPrecision")]
        public int BaseAssetPrecision { get; set; }
        
        /// <summary>
        /// Точность котировки инструмента
        /// </summary>
        [JsonProperty("quotePrecision")]
        public int QuotePrecision { get; set; }

        /// <summary>
        /// (?) Тип актива, на базе которого построен данный фьючерс
        /// </summary>
        [JsonProperty("underlyingType")]
        [JsonConverter(typeof(UnderlyingType), UnderlyingType.Unknown)]
        public UnderlyingType UnderlyingType { get; set; }

        /// <summary>
        /// (?) Подтип актива, на базе которого построен данный фьючерс
        /// </summary>
        [JsonProperty("underlyingSubType",
            ItemConverterType = typeof(StringEnumExConverter),
            ItemConverterParameters = new object[] { UnderlyingSubType.Unknown })]
        public UnderlyingSubType[] UnderlyingSubTypes { get; set; }

        /// <summary>
        /// "settlePlan": 0,
        /// </summary>
        [JsonProperty("settlePlan")]
        public int SettlePlan { get; set; }

        /// <summary>
        /// Threshold for algo order with "priceProtect"
        /// "triggerProtect": "0.15",
        /// </summary>
        [JsonProperty("triggerProtect")]
        public decimal TriggerProtect { get; set; }

        /// <summary>
        /// Фильтры
        /// </summary>
        [JsonProperty(PropertyName = "filters", ItemConverterType = typeof(FilterConverter))]
        public SymbolFilter[] Filters { get; set; }

        /// <summary>
        /// Допустимые типы ордеров
        /// </summary>
        [JsonProperty("OrderType",
            ItemConverterType = typeof(StringEnumExConverter),
            ItemConverterParameters = new object[] { OrderType.Unknown })]
        public OrderType[] OrderTypes { get; set; }

        /// <summary>
        /// Допустимые типы ордеров
        /// </summary>
        [JsonProperty("timeInForce",
            ItemConverterType = typeof(StringEnumExConverter),
            ItemConverterParameters = new object[] { BinanceApi.Contracts.Enums.TimeInForce.Unknown })]
        public TimeInForce[] TimeInForce { get; set; }

        /// <inheritdoc />
        public object Clone()
        {
            return new UFInstrument
            {
                Symbol = Symbol,
                Status = Status,
                MainMarginPercent = MainMarginPercent,
                RequireMarginPercent = RequireMarginPercent,
                BaseAsset = BaseAsset,
                QuoteAsset = QuoteAsset,
                MarginAsset = MarginAsset,
                PricePrecision = PricePrecision,
                QuantityPrecision = QuantityPrecision,
                BaseAssetPrecision = BaseAssetPrecision,
                QuotePrecision = QuotePrecision,
                UnderlyingType = UnderlyingType,
                UnderlyingSubTypes = UnderlyingSubTypes?.ToArray(),
                SettlePlan = SettlePlan,
                TriggerProtect = TriggerProtect,
                Filters = Filters?.Select(x => (SymbolFilter)x.Clone()).ToArray(),
                OrderTypes = OrderTypes?.ToArray(),
                TimeInForce = TimeInForce?.ToArray(),
            };
        }
    }


    /// <summary>
    /// (?) Тип актива, на базе которого построен данный фьючерс
    /// </summary>
    public enum UnderlyingType
    {
        /// <summary>
        /// Неизвестный (ошибочный) статус
        /// </summary>
        Unknown,

        /// <summary>
        /// Монета
        /// </summary>
        [EnumMember(Value = "COIN")]
        Coin,
    }

    /// <summary>
    /// (?) Подтип актива, на базе которого построен данный фьючерс
    /// </summary>
    public enum UnderlyingSubType
    {
        /// <summary>
        /// Неизвестный (ошибочный) статус
        /// </summary>
        Unknown,

        /// <summary>
        /// Монета
        /// </summary>
        [EnumMember(Value = "STORAGE")]
        Storage,
    }
}
