using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class ShoppingCartSummary: BaseSummary
    {
        /// <summary>
        /// 购物车
        /// </summary>
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

        public string Description { get; set; }
    }
}
