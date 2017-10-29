using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class BaseSummary
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 起始行号
        /// </summary>
        public int RowFrom { get; set; }

        /// <summary>
        /// 结束行号
        /// </summary>
        public int RowTo { get; set; }
    }
}
