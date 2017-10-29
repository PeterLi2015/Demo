using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理表
    /// </summary>
    [Table("UserRoles")]
    public class UserRoleEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }

       

    }
}
