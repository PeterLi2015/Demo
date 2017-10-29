using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DuoBang.SC.DataAccess.Interface;

namespace DuoBang.Model
{
    public class Department : IDeleteStatus
    {
        /// <summary>
        /// 部门在数据库类的编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 部门删除状态
        /// </summary>
        public int EntityStatus { get; set; }
    }
}