using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class MemberProduct : BaseModel<Guid>
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid MemberID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 会员
        /// </summary>
        public Member Member { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public Product Product { get; set; }
    }
}
