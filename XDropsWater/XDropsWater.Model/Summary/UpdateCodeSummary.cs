using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class UpdateCodeSummary: BaseSummary
    {
        /// <summary>
        /// 识别码列表
        /// </summary>
        public IEnumerable<UpdateCode> UpdateCodeList { get; set; }

    }
}
