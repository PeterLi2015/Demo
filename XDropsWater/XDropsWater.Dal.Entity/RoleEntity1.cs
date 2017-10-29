using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DuoBang.Dal.DataAccess.Entity;
using DuoBang.SC.DataAccess.Configuration;

namespace DuoBang.Dal.Entity
{
    /// <summary>
    /// 角色实体类
    /// </summary>
    [Table("Roles")]
    public class RoleEntity : BaseEntity<Guid>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [MaxLength(20), Required]
        public string Name { get; set; }

        /// <summary>
        /// 角色权限
        /// </summary>
        public virtual List<PermissionEntity> Permissions { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// 角色对应多个用户
        /// </summary>
        public virtual ICollection<UserEntity> Users { get; set; }

        protected override void ConfigMap(IEntityTypeConfiguration config)
        {
            base.ConfigMap(config);
            var tConfig = config as ExtendEntityTypeConfiguration<RoleEntity, Guid>;

            #region relationship between Roles and Users
            tConfig.HasMany(e => e.Users).WithMany(e => e.Roles)
                .Map(m => {
                    m.ToTable("RoleUserRel");
                    m.MapLeftKey("RoleID");
                    m.MapRightKey("UserID");
                });
            #endregion

            #region relationship between Roles and Permissions
            tConfig.HasMany(e => e.Permissions).WithMany(e => e.Roles)
                .Map(m =>
                {
                    m.ToTable("RolePermissionRel");
                    m.MapLeftKey("RoleID");
                    m.MapRightKey("PermissionID");
                });
            #endregion
        }

    }
}
