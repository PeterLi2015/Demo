using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DuoBang.Dal.DataAccess.Entity;
using DuoBang.SC.DataAccess.Interface;

namespace DuoBang.Dal.Entity
{
    /// <summary>
    /// 部门实体类
    /// </summary>
    [Table("Departments")]
    public class DepartmentEntity : BaseEntity<Guid>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// 部门删除状态
        /// </summary>
        public int EntityStatus { get; set; }

        /// <summary>
        /// 一个部门拥有多个角色
        /// </summary>
        public virtual ICollection<UserEntity> Users { get; set; }
    }
}
