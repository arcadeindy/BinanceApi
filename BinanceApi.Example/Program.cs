using System;
using NLog;
using PoissonSoft.BinanceApi;

namespace BinanceApi.Example
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ICredentialsProvider credentialsProvider = new NppCryptProvider();
            BinanceApiClientCredentials credentials;
            try
            {
                credentials = credentialsProvider.GetCredentials();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            var apiClient = new BinanceApiClient(credentials, logger) {IsDebug = true};

            new ActionManager(apiClient).Run();
        }
    }
}
