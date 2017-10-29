using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 用户操作日志实体类
    /// </summary>
    [Table("UserLog")]
    public class UserLogEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 操作人员账号
        /// </summary>
        [MaxLength(50), Required]
        public string Account { get; set; }

        /// <summary>
        /// 操作人员姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        [Required]
        public int Operation { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        [MaxLength(200)]
        public string Comment { get; set; }
    }
}
