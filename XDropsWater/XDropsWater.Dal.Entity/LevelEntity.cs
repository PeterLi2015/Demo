using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理级别
    /// </summary>
    [Table("Levels")]
    public class LevelEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
       
        /// <summary>
        /// 级别名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 个人消费折扣
        /// </summary>
        public decimal Discount { get; set; }

        
    }
}
