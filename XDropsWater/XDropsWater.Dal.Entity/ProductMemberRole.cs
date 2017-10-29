using XDropsWater.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XDropsWater.Dal.Entity
{
    [Table("ProductMemberRole")]
    public class ProductMemberRoleEntity : BaseEncryptCreateUpdateLogEntity<int>
    {
        public int ProductID { get; set; }

        public int MemberRoleID { get; set; }

        /// <summary>
        /// 角色价格
        /// </summary>
        public decimal RolePrice { get; set; }

        public int OneTimeAmount { get; set; }

        public virtual ProductEntity Product { get; set; }

        public virtual MemberRoleEntity MemberRole { get; set; }
    }
}
