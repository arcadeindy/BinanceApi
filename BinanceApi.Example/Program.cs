using System;
using PoissonSoft.BinanceApi;

namespace BinanceApi.Example
{
    class Program
    {
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

        }
    }
}
