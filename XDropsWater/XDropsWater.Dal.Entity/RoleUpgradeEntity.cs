using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;
using System.Collections.Generic;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理角色升级表
    /// </summary>
    [Table("RoleUpgrade")]
    public class RoleUpgradeEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid MemberId { get; set; } 
        public int OriginalRoleId { get; set; }
        public int CurrentRoleId { get; set; }
    }
}
