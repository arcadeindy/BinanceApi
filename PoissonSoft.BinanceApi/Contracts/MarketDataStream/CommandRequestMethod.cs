using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PoissonSoft.BinanceApi.Contracts.MarketDataStream
{
    /// <summary>
    /// Command Request Methods
    /// </summary>
    public enum CommandRequestMethod
    {
        /// <summary>
        /// Unknown (erroneous)
        /// </summary>
        Unknown,

        /// <summary>
        /// Subscribe stream
        /// </summary>
        [EnumMember(Value = "SUBSCRIBE")]
        Subscribe,

        /// <summary>
        /// Unsubscribe stream
        /// </summary>
        [EnumMember(Value = "UNSUBSCRIBE")]
        Unsubscribe,

        /// <summary>
        /// Request List of Subscriptions
        /// </summary>
        [EnumMember(Value = "LIST_SUBSCRIPTIONS")]
        ListSubscriptions,

        /// <summary>
        /// Set property
        /// </summary>
        [EnumMember(Value = "SET_PROPERTY")]
        SetProperty,

        /// <summary>
        /// Get property
        /// </summary>
        [EnumMember(Value = "GET_PROPERTY")]
        GetProperty,
    }
}
