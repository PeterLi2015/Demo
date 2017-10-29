using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 子订单表
    /// </summary>
    [Table("ChildOrders")]
    public class ChildOrderEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        [Required]
        public Guid MemberID { get; set; }

        /// <summary>
        /// 订货数量
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// 是否发货
        /// </summary>
        //[Required]
        public bool IsDeliverly { get; set; }

        /// <summary>
        /// 发货代理，表示是谁发货的
        /// </summary>
        public Guid SendMemberID { get; set; }


    }
}
