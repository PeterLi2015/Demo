using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 订单明细状态
    /// </summary>
    public enum OrderDetailsStatus
    {
        /// <summary>
        /// 没有识别码
        /// </summary>
        NoCode,
        /// <summary>
        /// 识别码未填满
        /// </summary>
        CodeNotFull,
        /// <summary>
        /// 识别码已填满
        /// </summary>
        CodeFull
    }
}
