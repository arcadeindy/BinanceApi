using Newtonsoft.Json;

namespace PoissonSoft.BinanceApi.Contracts.Filters
{
    /// <summary>
    /// The MAX_POSITION filter defines the allowed maximum position an account can have on the base asset of a symbol.
    /// An account's position defined as the sum of the account's:
    ///   - free balance of the base asset
    ///   - locked balance of the base asset
    ///   - sum of the qty of all open BUY orders
    /// BUY orders will be rejected if the account's position is greater than the maximum position allowed.
    /// </summary>
    public class SymbolFilterMaxPosition: SymbolFilter
    {
        /// <summary>
        /// The allowed maximum position an account can have on the base asset of a symbol.
        /// </summary>
        [JsonProperty("maxPosition")]
        public decimal MaxPosition { get; set; }

        /// <inheritdoc />
        protected override SymbolFilter CreateInstanceForClone()
        {
            return new SymbolFilterMaxPosition
            {
                FilterType = FilterType,
                MaxPosition = MaxPosition,
            };
        }
    }
}
