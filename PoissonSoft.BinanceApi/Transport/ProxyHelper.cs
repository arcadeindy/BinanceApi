using System;
using System.Net;

namespace PoissonSoft.BinanceApi.Transport
{
    internal static class ProxyHelper
    {
        public static IWebProxy CreateProxy(BinanceApiClientCredentials credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.ProxyAddress)) return null;
            var res = new WebProxy
            {
                Address = new Uri($"http://{credentials.ProxyAddress}"),
            };

            if (string.IsNullOrWhiteSpace(credentials.ProxyCredentials)) return res;

            var arr = credentials.ProxyCredentials.Split('@');
            if (arr.Length != 2)
                throw new Exception("Авторизационные данные прокси-сервера заданы некорректно. " +
                                    "Используйте строку следующего формата LOGIN@PASSWORD");

            res.Credentials = new NetworkCredential(arr[0], arr[1]);
            return res;
        }
    }
}
