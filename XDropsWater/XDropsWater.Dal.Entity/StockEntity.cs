using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// table of stock
    /// </summary>
    [Table("Stock")]
    public class StockEntity: BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid MemberID { get; set; }

        public int ProductID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }

        public virtual MemberEntity Member { get; set; }

        public virtual ProductEntity Product { get; set; }
    }
}
