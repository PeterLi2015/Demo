using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model.Register
{
    public class Member
    {
        public Guid MemberId { get; set; }
        public string Mobile { get; set; }
        public string MemberName { get; set; }
        public string Address { get; set; }
        public string IdentityNo { get; set; }
        public string Password { get; set; }
        public Guid ParentId { get; set; }
        public string ParentMobile { get; set; }
        public string ParentMemberName { get; set; }
        public int ProvinceAvailable { get; set; }
        public int GeneralAvailable { get; set; }
        public int RoleId { get; set; }
        public decimal CurrentRoleAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
