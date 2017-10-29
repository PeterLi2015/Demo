using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Interface;
using System.Threading.Tasks;


namespace XDropsWater.Model
{
    public class UserLog
    {
        /// <summary>
        /// 用户日志编号
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 用户ID编号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户ID编号
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 用户日志操作内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 用户日志操作时间
        /// </summary>
        public DateTime CreateOn { get; set; }
    }
}
