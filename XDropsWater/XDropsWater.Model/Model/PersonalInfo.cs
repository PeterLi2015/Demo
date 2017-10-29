using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class PersonalInfo: BaseModel<Guid>
    {
        public string MemberName { get; set; }
        public string Mobile { get; set; }
        public string IdentityNo { get; set; }
        /// <summary>
        /// 升级进货量
        /// </summary>
        public decimal UpgradeQuantity { get; set; }
        /// <summary>
        /// 优惠数量
        /// </summary>
        public int ReducedQuantity { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 用户角色ID
        /// </summary>
        public int UserRoleID { get; set; }
        /// <summary>
        /// 会员角色ID
        /// </summary>
        public int MemberRoleID { get; set; }
        public MemberRole MemberRole { get; set; }
    }
}
