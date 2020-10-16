using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_NUM_ICEBERG_ORDERS filter defines the maximum number of ICEBERG orders an account is allowed to have open on a symbol.
    /// An ICEBERG order is any order where the icebergQty is > 0.
    /// </summary>
    public class SymbolFilterMaxNumIcebergOrders : SymbolFilter
    {
        /// <summary>
        /// Maximum number of ICEBERG orders an account is allowed to have open on a symbol
        /// </summary>
        [JsonProperty("maxNumIcebergOrders")]
        public int MaxNumIcebergOrders { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterMaxNumIcebergOrders
            {
                FilterType = FilterType,
                MaxNumIcebergOrders = MaxNumIcebergOrders,
            };
        }
    }
}
