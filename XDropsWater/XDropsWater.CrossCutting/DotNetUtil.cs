using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace XDropsWater.CrossCutting
{
    /// <summary>
    /// 辅助通用类
    /// </summary>
    public class DotNetUtil
    {
        /// <summary>
        /// 泛型比较辅助方法 如果T为引用类型Null为最小值
        /// </summary>
        /// <typeparam name="T">比较数类型</typeparam>
        /// <param name="t1">左比较数</param>
        /// <param name="t2">右比较数</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此实例小于 obj。零此实例等于 obj。大于零此实例大于 obj。</returns>
        public static int Compare<T>(T t1, T t2) where T : IComparable
        {
            if (!typeof(T).IsValueType)
            {
                if (t1 == null && t2 == null) return 0;
                if (t1 == null) return -1;
            }
            return t1.CompareTo(t2);
        }

        /// <summary>
        /// 是否为Http网页请求
        /// </summary>
        /// <returns></returns>
        public static bool IsHttpRequest()
        {
            return System.Web.HttpContext.Current != null;
        }
    }
}
