using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 识别码状态
    /// </summary>
    public enum CodeStatus
    {
        /// <summary>
        /// 可用
        /// </summary>
        Available,
        /// <summary>
        /// 不可用，但下家还没发货
        /// </summary>
        ChildNotSend,
        /// <summary>
        /// 不可用，下家已经发货
        /// </summary>
        NotAvailable
    }
}
