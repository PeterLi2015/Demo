using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// product summary object 
    /// </summary>
    public class ProductSummary : BaseSummary
    {
        /// <summary>
        /// list of products
        /// </summary>
        public IEnumerable<Product> Products { get; set; }
    }
}
