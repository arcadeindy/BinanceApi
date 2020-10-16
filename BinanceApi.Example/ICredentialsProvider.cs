using System;
using System.Collections.Generic;
using System.Text;
using PoissonSoft.BinanceApi;

namespace BinanceApi.Example
{
    interface ICredentialsProvider
    {
        BinanceApiClientCredentials GetCredentials();
    }
}
