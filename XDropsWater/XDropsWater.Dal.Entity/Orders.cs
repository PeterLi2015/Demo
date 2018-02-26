using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理表
    /// </summary>
    [Table("Orders")]
    public class OrderEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// No of the order
        /// </summary>
        [MaxLength(50)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 代理ID
        /// </summary>
        [Required]
        public Guid MemberID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 分成数量(总代分成数量)
        /// </summary>
        public Nullable<int> BonusQuantity { get; set; }

        /// <summary>
        /// 是否发货
        /// </summary>
        [Required]
        public bool IsDeliverly { get; set; }

        /// <summary>
        /// the ID of the member who sent the order
        /// </summary>
        public Guid? SendMemberID { get; set; }

        /// <summary>
        /// the description of the order
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// total amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 快递信息
        /// </summary>
        public string Express { get; set; }

        public Nullable<Guid> GeneralAgent1ID { get; set; }
        public Nullable<Guid> GeneralAgent2ID { get; set; }
        public Nullable<Guid> GeneralAgent3ID { get; set; }
        public Nullable<Guid> DirectorID { get; set; }
        /// <summary>
        /// 发货日期
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// 0不可发货(金额不够), 1没有识别码(可发货)，2有识别码未填满(不可发货)，3有识别码已填满(可发货)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 0未收款，1已收款
        /// </summary>
        public int FinancialStatus { get; set; }


        public virtual MemberEntity Member { get; set; }

        /// <summary>
        /// send member of the order
        /// </summary>
        public virtual MemberEntity SendMember { get; set; }

    }
}
