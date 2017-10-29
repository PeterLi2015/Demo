using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDropsWater.Model
{
    public class OpResult
    {
        /// <summary>
        /// 错误码,0表示成功，其他表示失败
        /// </summary>
        public ErrorCodes ErrCode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}