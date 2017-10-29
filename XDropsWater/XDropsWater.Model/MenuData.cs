using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDropsWater.Model
{
    public class MenuItem
    {
        public MenuItem()
        {
            Url = "/Home/NotSupport";
        }

        /// <summary>
        /// 菜单编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜单名称（中文）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 该菜单要打开的具体页面
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }
    }

    public class MenuGroup
    {
        /// <summary>
        /// 菜单分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组下面的菜单列表
        /// </summary>
        public List<MenuItem> MenuItems { get; set; }
    }
}