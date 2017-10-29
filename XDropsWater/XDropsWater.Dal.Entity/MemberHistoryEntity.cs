using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理历史表
    /// </summary>
    [Table("MembersHistory")]
    public class MemberHistoryEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// Member表主键
        /// </summary>
        [Required]
        public Guid MemberID { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [Required, MaxLength(50)]
        public string IdentityCardNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required, MaxLength(50)]
        public string MemberName { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        //[Required]
        public Guid? ParentMemberID { get; set; }

        /// <summary>
        /// 推荐人ID
        /// </summary>
        public Guid? RecommendMemberID { get; set; }

        /// <summary>
        /// 部门A代理ID
        /// </summary>
        public Guid? DepartAMemberID { get; set; }

        /// <summary>
        /// 部门A积分
        /// </summary>
        public int? DepartAScore { get; set; }

        /// <summary>
        /// A部门区以上代理人数
        /// </summary>
        public int? DepartADistrictAgencyQuantity { get; set; }

        /// <summary>
        /// 部门B代理ID
        /// </summary>
        public Guid? DepartBMemberID { get; set; }

        /// <summary>
        /// 部门B积分
        /// </summary>
        public int? DepartBScore { get; set; }

        /// <summary>
        /// B部门区以上代理人数
        /// </summary>
        public int? DepartBDistrictAgencyQuantity { get; set; }

        /// <summary>
        /// 部门C代理ID
        /// </summary>
        public Guid? DepartCMemberID { get; set; }

        /// <summary>
        /// 部门C积分
        /// </summary>
        public int? DepartCScore { get; set; }

        /// <summary>
        /// C部门区以上代理人数
        /// </summary>
        public int? DepartCDistrictAgencyQuantity { get; set; }

        /// <summary>
        /// 代理卡级别(1银卡2金卡3钻卡)
        /// </summary>
        public int? CardLevelID { get; set; }

        /// <summary>
        /// 代理级别(1区代理2市代理3省代理)
        /// </summary>
        public int? AgencyLevelID { get; set; }

        //public virtual MemberEntity ParentMember { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        [MaxLength(50)]
        public string BankCardNo { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [MaxLength(50)]
        public string BankName { get; set; }

        /// <summary>
        /// 是否新人
        /// </summary>
        [Required]
        public bool IsNew { get; set; }

        /// <summary>
        /// 直接上的高号是什么级别，0，没有直接上高号
        /// </summary>
        public int HighNumberCardLevelID { get; set; }

        /// <summary>
        /// 是否允许更改
        /// </summary>
        [Required]
        public bool AllowUpdate { get; set; }

        /// <summary>
        /// 身份证是否核对过
        /// </summary>
        public bool? IDCardHasChecked { get; set; }

        /// <summary>
        /// 核对人ID
        /// </summary>
        public Guid? CheckBy { get; set; }

        /// <summary>
        /// 核对时间
        /// </summary>
        public DateTime? CheckOn { get; set; }
        
        /// <summary>
        /// 应返还业绩
        /// </summary>
        public decimal? MustReturnAchieve { get; set; }

        /// <summary>
        /// 已返还业绩
        /// </summary>
        public decimal? AlreadyReturnedAchieve { get; set; }

        /// <summary>
        /// 自己的积分
        /// </summary>
        public int? PersonalScore { get; set; }
    }
}
