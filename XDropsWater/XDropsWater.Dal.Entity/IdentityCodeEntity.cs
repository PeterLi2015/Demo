using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// entity of product identity code
    /// </summary>
    [Table("IdentityCode")]
    public class IdentityCodeEntity: BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// the order details ID
        /// </summary>
        public Guid? OrderDetailsID { get; set; }
        
        /// <summary>
        /// 唯一辨识码
        /// </summary>
        public long Code { get; set; }
        /// <summary>
        /// 0可用，1给下家用(下家还没发货)，2不可用(下家已经发货)，3失效(已经到消费者手里)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// order details
        /// </summary>
        public virtual OrderDetailsEntity OrderDetails { get; set; }
    }
}
