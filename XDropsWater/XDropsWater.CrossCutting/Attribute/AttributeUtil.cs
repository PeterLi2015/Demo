using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Attribute
{
    /// <summary>
    /// 操作或读取自定义特性
    /// </summary>
    public class AttributeUtil
    {
        /// <summary>
        /// 获得类指定特性对象数组
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="type">类的类型</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性。</param>
        /// <returns>返回特性对象数组</returns>
        public static TAttribute[] GetAttrs<TAttribute>(Type type, bool inherit)
            where TAttribute : class
        {
            var objs = type.GetCustomAttributes(typeof(TAttribute), inherit);
            return Array.ConvertAll(objs, new Converter<object, TAttribute>(o =>
            {
                return o as TAttribute;
            }));
        }
        /// <summary>
        /// 获得类指定特性第一个对象实例
        /// </summary>
        /// <typeparam name="TAttribute">特性类型</typeparam>
        /// <param name="type">类的类型</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性。</param>
        /// <returns>返回特性对象实例</returns>
        public static TAttribute GetFirstAttr<TAttribute>(Type type, bool inherit)
            where TAttribute : class
        {
            var attrs = GetAttrs<TAttribute>(type, inherit);
            if (attrs == null || attrs.Length == 0) return null;
            return attrs[0];
        }
    }
}
