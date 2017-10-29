using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理角色表
    /// </summary>
    [Table("MemberRoles")]
    public class MemberRoleEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        /// <summary>
        /// 一次最少进货多少钱
        /// </summary>
        [Required]
        public int OneTimeAmount { get; set; }

        public string RoleRiseDescription { get; set; }

        public bool AllowedDirectOrder { get; set; }

        public int Price { get; set; }

        /// <summary>
        /// 总共需要多少进货量可以达到该级别
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 升级数量
        /// </summary>
        public int UpgradeCount { get; set; }

        

    }
}
