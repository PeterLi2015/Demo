using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class BaseInfo
    {
        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public string Address { get; set; }

        public string IdentityNo { get; set; }

        public int UserRoleID { get; set; }

        public MemberRole MemberRole { get; set; }

        /// <summary>
        /// 升级金额
        /// </summary>
        public decimal UpgradeAmount { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal MinusAmount { get; set; }
    }
}
