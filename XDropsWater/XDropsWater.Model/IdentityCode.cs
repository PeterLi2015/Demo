using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class IdentityCode : BaseModel<Guid>
    {
        public long CodeFrom { get; set; }

        public long CodeTo { get; set; }

        /// <summary>
        /// 订单明细ID
        /// </summary>
        public Guid? OrderDetailsID { get; set; }

        /// <summary>
        /// 唯一辨识码
        /// </summary>
        public long Code { get; set; }
        /// <summary>
        /// 0可用，1不可用，2失效
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 订单明细
        /// </summary>
        public OrderDetails OrderDetails { get; set; }

        public bool Checked { get; set; }
    }
}
