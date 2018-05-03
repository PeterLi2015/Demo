using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 总代订单
    /// </summary>
    public class GeneralOrderModel : BaseModel<Guid>
    {
        /// <summary>
        /// 总代名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
