using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DuoBang.Dal.DataAccess.Entity;

namespace DuoBang.Dal.Entity
{
    /// <summary>
    /// 权限实体类
    /// </summary>
    [Table("Permissions")]
    public class PermissionEntity : BaseEntity<Guid>
    {
        /// <summary>
        /// 权限短名称
        /// </summary>
        [MaxLength(50)]
        public string ShortName { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 权限对应多个角色
        /// </summary>
        public List<RoleEntity> Roles { get; set; }

    }
}
