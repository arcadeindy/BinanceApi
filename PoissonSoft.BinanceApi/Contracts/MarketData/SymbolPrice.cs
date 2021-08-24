using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi.Contracts.MarketData
{
    /// <summary>
    /// Цена по валютной паре
    /// </summary>
    public class SymbolPrice
    {
        /// <summary>
        /// Валютная пара
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
    }
}
