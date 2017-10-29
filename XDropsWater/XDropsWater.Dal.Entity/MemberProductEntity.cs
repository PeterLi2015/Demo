using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 会员产品表
    /// </summary>
    [Table("MemberProduct")]
    public class MemberProductEntity: BaseEncryptCreateUpdateLogEntity<Guid>
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
        public virtual MemberEntity Member { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        public virtual ProductEntity Product { get; set; }
    }
}
