using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class MemberProductSummary: BaseSummary
    {
        /// <summary>
        /// 会员库存
        /// </summary>
        public IEnumerable<MemberProduct> MemberProducts { get; set; }
    }
}
