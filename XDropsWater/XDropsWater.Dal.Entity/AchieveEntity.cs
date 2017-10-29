using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 业绩表
    /// </summary>
    [Table("Achieves")]
    public class AchieveEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        [Required]
        public Guid MemberID { get; set; }

        public virtual MemberEntity Member { get; set; }

        /// <summary>
        /// 业绩金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 是否提交
        /// </summary>
        public bool IsSubmit { get; set; }

        /// <summary>
        /// 业绩类型(1新代理入单2老代理补单3老代理个人消费)
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int? BatchNumber { get; set; }

        public int? ProductID { get; set; }

        public virtual ProductEntity Product { get; set; }

        /// <summary>
        /// 积分，2800为1分
        /// </summary>
        public int? Score { get; set; }

        // <summary>
        // 必须消费
        // </summary>
        //public decimal? MustConsume { get; set; }

        ///// <summary>
        ///// 计算奖金的业绩(超出个人消费部分只算一半)
        ///// </summary>
        //public decimal? BonusAchieve { get; set; }

        ///// <summary>
        ///// 实际业绩，主要是针对老代理个人消费，如果超过必须消费，则超过部分算一般业绩
        ///// </summary>
        //public decimal? ActualAchieve { get; set; }
    }
}
