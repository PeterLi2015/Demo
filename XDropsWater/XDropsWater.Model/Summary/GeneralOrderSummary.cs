using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class GeneralOrderSummary: BaseSummary
    {
        /// <summary>
        /// 总代订单列表
        /// </summary>
        public IEnumerable<GeneralOrderModel> GeneralOrderList { get; set; }

    }
}
