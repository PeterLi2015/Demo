using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class ProductMemberRole : BaseModel<int>
    {
        public int ProductID { get; set; }

        public int MemberRoleID { get; set; }

        /// <summary>
        /// 角色价格
        /// </summary>
        public decimal RolePrice { get; set; }

        public Product Product { get; set; }

        public MemberRole MemberRole { get; set; }

        /// <summary>
        /// quantity
        /// </summary>
        public int Quantity { get; set; }
    }
}
