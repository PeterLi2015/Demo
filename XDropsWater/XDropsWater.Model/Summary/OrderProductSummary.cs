using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// order product summary
    /// </summary>
    public class OrderProductSummary: BaseSummary
    {
        public IEnumerable<OrderProduct> Products { get; set; }
    }
}
