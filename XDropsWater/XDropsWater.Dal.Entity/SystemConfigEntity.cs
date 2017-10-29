using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using XDropsWater.DataAccess.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.Dal.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 系统配置项实体类
    /// </summary>
    [Table("SystemConfig")]
    public class SystemConfigEntity : BaseEntity<Guid>
    {
        /// <summary>
        /// 配置项名称
        /// </summary>
        [Required, MaxLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 配置项具体值
        /// </summary>
        [Required]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 配置项描述
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// 配置项删除状态
        /// </summary>
        public int EntityStatus { get; set; }

    }
}
