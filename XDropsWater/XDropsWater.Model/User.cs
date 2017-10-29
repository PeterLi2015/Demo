using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XDropsWater.Model
{
    public class UserSummary
    {
        /// <summary>
        /// 角色主键
        /// </summary>
        public Guid ID { get; set; }

        public Guid MemberID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 用户帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        public int UserRoleID { get; set; }

        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public Guid ParentMemberID { get; set; }

        public int RoleID { get; set; }

        public int PreviousRoleID { get; set; }

        public int PreviousRoleQuantity { get; set; }

        public string Address { get; set; }

        public int TotalQuantity { get; set; }

        public int ChildTotalQuantity { get; set; }

        public int CurrentRoleQuantity { get; set; }
        
    }
    public class UserDetail
    {
        /// <summary>
        /// 角色主键
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 用户帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户手机
        /// </summary>
        public string MobTel { get; set; }

        /// <summary>
        /// 用户座机号
        /// </summary>
        public string Telphone { get; set; }

        //[MaxLength(20)]
        public string StoreManagerName { get; set; }

        //[MaxLength(200)]
        public string Address { get; set; }

        public string Fax { get; set; }

        public int RoleID { get; set; }

        ///// <summary>
        ///// 用户所属网点Id
        ///// </summary>
        //public Guid DepartmentId { get; set; }

        ///// <summary>
        ///// 用户所属网点名称
        ///// </summary>
        //public string DepartmentName { get; set; }

        ///// <summary>
        ///// 角色权限
        ///// </summary>
        //public List<RoleSummary> Roles { get; set; }
    }

    public enum enmRoles
    {
        All = 0,
        Admin = 1,
        General = 2
    }
    public enum enmMemberRole
    {
        [Display(Name = "顾客")]
        Customer = 1,
        [Display(Name = "一级代理")]
        Agent1 = 2,
        [Display(Name = "二级代理")]
        Agent2 = 3,
        [Display(Name = "县级代理")]
        County = 4,
        [Display(Name = "市代理")]
        City = 5,
        [Display(Name = "省代理")]
        Province = 6,
        [Display(Name = "总代理")]
        GeneralAgent = 7,
        [Display(Name = "董事")]
        Director = 8
    }
}