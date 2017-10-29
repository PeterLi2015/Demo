using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class IdentityCodeSummary : BaseSummary
    {
        /// <summary>
        /// 编号列表
        /// </summary>
        public IEnumerable<IdentityCode> IdentityCodes { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 订购数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否发货
        /// </summary>
        public bool IsDeliverly { get; set; }

    }
}
