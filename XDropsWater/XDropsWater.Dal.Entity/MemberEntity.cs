using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理表
    /// </summary>
    [Table("Members")]
    public class MemberEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 身份证号码
        /// </summary>
         [MaxLength(20)]
        public string IdentityNo { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required, StringLength(20)]
        public string Mobile { get; set; }
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
        /// 代理角色ID(0公司,1以上代理)
        /// </summary>
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public virtual MemberRoleEntity Role { get; set; }

        /// <summary>
        /// 代理上一次角色
        /// </summary>
        public int PreviousRoleID { get; set; }

        public int PreviousRoleQuantity { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal TotalQuantity { get; set; }

        /// <summary>
        /// 底下代理总件数
        /// </summary>
        public int ChildTotalQuantity { get; set; }

        /// <summary>
        /// 当前级别总共拿了多少件货
        /// </summary>
        public decimal CurrentRoleQuantity { get; set; }

        public int ValidRole { get; set; }

        /// <summary>
        /// 有效省代
        /// </summary>
        public int ProvinceAvailable { get; set; }

        /// <summary>
        /// 有效总代
        /// </summary>
        public int GeneralAvailable { get; set; }

        /// <summary>
        /// 总进货量
        /// </summary>
        public decimal TotalCount { get; set; }

        /// <summary>
        /// 上董事日期
        /// </summary>
        public DateTime? DirectorDate { get; set; }

        /// <summary>
        /// 董事数量
        /// </summary>
        public int DirectorCount { get; set; }

        public virtual MemberEntity ParentMember { get; set; }

        /// <summary>
        /// 当前角色总金额
        /// </summary>
        public decimal CurrentRoleAmount { get; set; }

        /// <summary>
        /// 总进货金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 市代升省代可以减掉多少数量
        /// </summary>
        public int CityMinus { get; set; }

    }
}
