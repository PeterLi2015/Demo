using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Table("Users")]
    public class UserEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public Guid? MemberID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [Required, MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(50)]
        public string Password { get; set; }

        /// <summary>
        /// 用户角色ID
        /// </summary>
        [Required]
        public int UserRoleID { get; set; }

    }
}
