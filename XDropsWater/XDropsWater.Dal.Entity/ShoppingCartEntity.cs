using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 购物车
    /// </summary>
    [Table("ShoppingCart")]
    public class ShoppingCartEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public Guid MemberID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Quantity { get; set; }

        public virtual MemberEntity Member { get; set; }

        public virtual ProductEntity Product { get; set; }
    }
}
