namespace PoissonSoft.BinanceApi.UserDataStreams
{

    /// <summary>
    /// User Data Stream для спотового аккаунта
    /// </summary>
    public class SpotUserDataStream : UserDataStream
    {
        /// <inheritdoc />
        public SpotUserDataStream(BinanceApiClient apiClient) : base(apiClient)
        {
            StreamType = UserDataStreamType.Spot;
        }
    }
}
