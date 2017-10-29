using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 代理角色配置表
    /// </summary>
    [Table("MemberRoleConfig")]
    public class MemberRoleConfigEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
        /// <summary>
        /// 代理角色ID
        /// </summary>
        public int MemberRoleID { get; set; }

        /// <summary>
        /// 子代理角色ID
        /// </summary>
        [Required, MaxLength(50)]
        public string ChildMemberRoleID { get; set; }

        /// <summary>
        /// 子代理个数
        /// </summary>
        //[Required]
        public int ChildQuantity { get; set; }

       

    }
}
