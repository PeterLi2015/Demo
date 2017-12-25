using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class ExpressSummary: BaseSummary
    {
        /// <summary>
        /// 快递列表
        /// </summary>
        public IEnumerable<Express> ExpressList { get; set; }

    }
}
