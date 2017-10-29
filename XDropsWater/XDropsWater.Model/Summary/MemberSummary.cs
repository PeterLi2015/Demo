using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 代理
    /// </summary>
    public class MemberSummary : BaseSummary
    {

        /// <summary>
        /// 代理列表
        /// </summary>
        public IEnumerable<Member> Members { get; set; }

    }
}
