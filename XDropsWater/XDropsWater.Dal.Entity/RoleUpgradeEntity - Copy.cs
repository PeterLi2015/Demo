using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HengtianSoft.SC.DataAccess.Entity;
using System.Collections.Generic;

namespace DuoBang.Dal.Entity
{
    /// <summary>
    /// 会员角色升级表
    /// </summary>
    [Table("RoleUpgrade")]
    public class RoleUpgradeEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid MemberId { get; set; }
        public int OriginalRoleId { get; set; }
        public int CurrentRoleId { get; set; }
    }
}
