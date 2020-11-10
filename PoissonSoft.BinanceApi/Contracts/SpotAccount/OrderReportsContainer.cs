using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.BinanceApi.Contracts.SpotAccount
{
    /// <summary>
    /// Container of OrderReport and OCOOrderReport
    /// </summary>
    public class OrderReportsContainer
    {
        /// <summary>
        /// Orders
        /// </summary>
        public ICollection<OrderReport> Orders { get; set; }
        
        /// <summary>
        /// OCO orders
        /// </summary>
        public ICollection<OCOOrderReport> OCOOrders { get; set; }
    }
}
