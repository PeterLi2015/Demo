using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 权限
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 代理管理添加权限
        /// </summary>
        public bool MemberManageAdd { get; set; }

        /// <summary>
        /// 代理管理操作权限
        /// </summary>
        public bool MemberManageOperate { get; set; }

        /// <summary>
        /// 代理订单管理操作权限
        /// </summary>
        public bool MemberOrderManageOperate { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 是否财务人员
        /// </summary>
        public bool IsFinancial { get; set; }
    }

    /// <summary>
    /// 订单级别
    /// </summary>
    public enum enmOrderLevel
    {
        /// <summary>
        /// 公司
        /// </summary>
        Company,

        /// <summary>
        /// 代理
        /// </summary>
        Agency
    }
}
