using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The ICEBERG_PARTS filter defines the maximum parts an iceberg order can have.
    /// The number of ICEBERG_PARTS is defined as CEIL(qty / icebergQty).
    /// </summary>
    public class SymbolFilterIcebergParts : SymbolFilter
    {
        /// <summary>
        /// Maximum parts an iceberg order can have
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterIcebergParts
            {
                FilterType = FilterType,
                Limit = Limit
            };
        }

    }
}
