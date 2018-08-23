using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 快递信息表
    /// </summary>
    [Table("Express")]
    public class ExpressEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid MemberID { get; set; }
        public virtual MemberEntity Member { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// 收件人手机号码
        /// </summary>
        public string RecipientMobile { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string RecipientAddress { get; set; }

        /// <summary>
        /// 快递名称
        /// </summary>
        public string ExpressName { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 发货内容
        /// </summary>
        [MaxLength(500)]
        public string Content { get; set; }

        /// <summary>
        /// 状态(0未发货,1已发货,2 部分发货)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 发货日期
        /// </summary>
        public Nullable<DateTime> ExpressDate { get; set; }

    }
}
