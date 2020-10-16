namespace PoissonSoft.BinanceApi.Transport
{
    /// <summary>
    /// Тип безопасности
    /// </summary>
    public enum EndpointSecurityType
    {
        /// <summary>
        /// Endpoint can be accessed freely.
        /// </summary>
        None,

        /// <summary>
        /// Endpoint requires sending a valid API-Key and signature.
        /// </summary>
        Trade,

        /// <summary>
        /// Endpoint requires sending a valid API-Key and signature.
        /// </summary>
        Margin,

        /// <summary>
        /// Endpoint requires sending a valid API-Key and signature.
        /// </summary>
        UserData,

        /// <summary>
        /// Endpoint requires sending a valid API-Key.
        /// </summary>
        UserStream,

        /// <summary>
        /// Endpoint requires sending a valid API-Key.
        /// </summary>
        MarketData
    }
}
