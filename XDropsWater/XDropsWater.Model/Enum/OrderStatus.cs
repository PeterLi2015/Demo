﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 没有识别码
        /// </summary>
        NoCode,
        /// <summary>
        /// 金额不够
        /// </summary>
        LessAmount,
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
