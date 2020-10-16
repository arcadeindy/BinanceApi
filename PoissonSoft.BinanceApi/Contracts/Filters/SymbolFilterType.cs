using System.Runtime.Serialization;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{

    /// <summary>
    /// Типы фильтров для торговых инструментов
    /// https://binance-docs.github.io/apidocs/spot/en/#filters
    /// </summary>
    public enum SymbolFilterType
    {
        /// <summary>
        /// Неизвестный (ошибочный) тип
        /// </summary>
        Unknown,

        /// <summary>
        /// The PRICE_FILTER defines the price rules for a symbol.
        /// </summary>
        [EnumMember(Value = "PRICE_FILTER")]
        PriceFilter,

        /// <summary>
        /// The PERCENT_PRICE filter defines valid range for a price based on the average of the previous trades.
        /// </summary>
        [EnumMember(Value = "PERCENT_PRICE")]
        PercentPrice,

        /// <summary>
        /// The LOT_SIZE filter defines the quantity (aka "lots" in auction terms) rules for a symbol.
        /// </summary>
        [EnumMember(Value = "LOT_SIZE")]
        LotSize,

        /// <summary>
        /// The MIN_NOTIONAL filter defines the minimum notional value allowed for an order on a symbol.
        /// An order's notional value is the price * quantity. If the order is an Algo order (e.g. STOP_LOSS_LIMIT),
        /// then the notional value of the stopPrice * quantity will also be evaluated. If the order is an Iceberg Order,
        /// then the notional value of the price * icebergQty will also be evaluated. 
        /// </summary>
        [EnumMember(Value = "MIN_NOTIONAL")]
        MinNotional,

        /// <summary>
        /// The ICEBERG_PARTS filter defines the maximum parts an iceberg order can have.
        /// The number of ICEBERG_PARTS is defined as CEIL(qty / icebergQty).
        /// </summary>
        [EnumMember(Value = "ICEBERG_PARTS")]
        IcebergParts,

        /// <summary>
        /// The MARKET_LOT_SIZE filter defines the quantity (aka "lots" in auction terms) rules for MARKET orders on a symbol.
        /// </summary>
        [EnumMember(Value = "MARKET_LOT_SIZE")]
        MarketLotSize,

        /// <summary>
        /// The MAX_NUM_ORDERS filter defines the maximum number of orders an account is allowed to have open on a symbol.
        /// Note that both "algo" orders and normal orders are counted for this filter.
        /// </summary>
        [EnumMember(Value = "MAX_NUM_ORDERS")]
        MaxNumOrders,

        /// <summary>
        /// The MAX_NUM_ALGO_ORDERS filter defines the maximum number of "algo" orders an account is allowed to have open on a symbol.
        /// "Algo" orders are STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.
        /// </summary>
        [EnumMember(Value = "MAX_NUM_ALGO_ORDERS")]
        MaxNumAlgoOrders,

        /// <summary>
        /// The MAX_NUM_ICEBERG_ORDERS filter defines the maximum number of ICEBERG orders an account is allowed to have open on a symbol.
        /// An ICEBERG order is any order where the icebergQty is > 0.
        /// </summary>
        [EnumMember(Value = "MAX_NUM_ICEBERG_ORDERS")]
        MaxNumIcebergOrders,

        /// <summary>
        /// The MAX_POSITION filter defines the allowed maximum position an account can have on the base asset of a symbol.
        /// An account's position defined as the sum of the account's:
        ///   - free balance of the base asset
        ///   - locked balance of the base asset
        ///   - sum of the qty of all open BUY orders
        /// BUY orders will be rejected if the account's position is greater than the maximum position allowed.
        /// </summary>
        [EnumMember(Value = "MAX_POSITION")]
        MaxPosition,

    }
}
