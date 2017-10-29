using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 代理
    /// </summary>
    public class Member : BaseModel<Guid>
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdentityNo { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public MemberRole Role { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 当前角色库存
        /// </summary>
        public int CurrentRoleQuantity { get; set; }

        /// <summary>
        /// 省代
        /// </summary>
        public int ProvinceAvailable { get; set; }

        /// <summary>
        /// 总代
        /// </summary>
        public int GeneralAvailable { get; set; }

        /// <summary>
        /// 上级代理ID
        /// </summary>
        public Guid ParentMemberID { get; set; }

        /// <summary>
        /// 上级代理
        /// </summary>
        public Member ParentMember { get; set; }

        /// <summary>
        /// 角色金额
        /// </summary>
        public decimal CurrentRoleAmount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

    }
}
