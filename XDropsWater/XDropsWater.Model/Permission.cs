
using System;
namespace DuoBang.Model
{
    public class Permission
    {
        /// <summary>
        /// 权限在数据库类的编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 权限短名称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 权限在数据库类的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限在数据库类的描述
        /// </summary>
        public string Description { get; set; }
    }
}
