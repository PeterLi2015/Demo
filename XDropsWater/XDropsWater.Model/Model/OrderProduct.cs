using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// product object is used for order
    /// </summary>
    public class OrderProduct : Product
    {
        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }
    }
}
