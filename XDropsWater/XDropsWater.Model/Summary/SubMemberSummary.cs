using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 代理
    /// </summary>
    public class SubMemberSummary : BaseSummary
    {

        /// <summary>
        /// 代理列表
        /// </summary>
        public IEnumerable<SubMemberModel> Members { get; set; }

        /// <summary>
        /// 第一层代理人数
        /// </summary>
        public int A { get; set; }

        /// <summary>
        /// 第二层代理人数
        /// </summary>
        public int B { get; set; }

        /// <summary>
        /// 三层即以下代理人数
        /// </summary>
        public int C { get; set; }

    }
}
