using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class ProductMemberRoleSummary : BaseSummary
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ProductMemberRole> Products { get; set; }
    }
}
