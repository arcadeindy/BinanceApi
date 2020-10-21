using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi.Contracts.Exceptions
{
    /// <summary>
    /// HTTP 429 return code is used when breaking a request rate limit.
    /// </summary>
    public class RequestRateLimitBreakingException: Exception
    {
        /// <inheritdoc />
        public RequestRateLimitBreakingException() : base() { }

        /// <inheritdoc />
        public RequestRateLimitBreakingException(string msg) : base(msg) { }

        /// <inheritdoc />
        public RequestRateLimitBreakingException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }
}
