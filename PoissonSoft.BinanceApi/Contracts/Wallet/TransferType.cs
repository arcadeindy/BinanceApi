namespace PoissonSoft.BinanceApi.Contracts.Wallet
{
    /// <summary>
    /// Transfer (deposit/withdraw) type
    /// </summary>
    public enum TransferType
    {
        /// <summary>
        /// External transfer
        /// </summary>
        External = 0,

        /// <summary>
        /// Internal transfer
        /// </summary>
        Internal = 1,
    }
}
