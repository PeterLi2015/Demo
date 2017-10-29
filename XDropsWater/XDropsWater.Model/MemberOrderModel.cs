using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class MemberOrderModel : BaseModel<Guid>
    {
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "姓名")]
        public string MemberName { get; set; }

        /// <summary>
        /// 订货数量
        /// </summary>
        [Display(Name = "订货数量")]
        public decimal Quantity { get; set; }

        public string Address { get; set; }

        public bool IsSubmit { get; set; }

        public int MemberRoleID { get; set; }

        public string SendMemberMobile { get; set; }
        public string SendMemberName { get; set; }

        //public int Price { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }

        [Display(Name = "快递信息")]
        public string Express { get; set; }
        public string MemberRoleName { get; set; }


    }

    /// <summary>
    /// 订单
    /// </summary>
    public class Order : BaseModel<Guid>
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订货数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 是否发货
        /// </summary>
        public bool IsDeliverly { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 快递
        /// </summary>
        public string Express { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否有唯一识别码
        /// </summary>
        public bool HasIdentityCode { get; set; }

        /// <summary>
        /// 代理
        /// </summary>
        public Member Member { get; set; }

        /// <summary>
        /// send member
        /// </summary>
        public Member SendMember { get; set; }

    }



    /// <summary>
    /// 角色
    /// </summary>
    public class MemberRole : BaseModel<int>
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 订单
    /// </summary>
    public class OrderSummary : BaseSummary
    {
        /// <summary>
        /// 订单列表
        /// </summary>
        public IEnumerable<Order> Orders { get; set; }

        /// <summary>
        /// 所有订单都已发货
        /// </summary>
        public bool AllDeliverly { get; set; }
    }


}
