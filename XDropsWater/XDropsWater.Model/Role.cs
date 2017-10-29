using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuoBang.Model
{
    public class RoleSummary
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色权限名称
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }


    }

    public class RoleDetail
    {
        /// <summary>
        /// 角色编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色权限名称
        /// </summary>
        public List<Permission> Permissions { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }
    }
}
