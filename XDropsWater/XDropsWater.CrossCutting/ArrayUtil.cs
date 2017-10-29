using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace XDropsWater.CrossCutting
{
    /// <summary>
    /// 数组的扩展功能
    /// </summary>
    public class ArrayUtil
    {
        /// <summary>
        /// 是否为空数组
        /// </summary>
        public static bool IsEmptyArray(ICollection ie)
        {
            return ie == null || !ie.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// 转化数组类型
        /// </summary>
        public static Array ChangeArrayType(Array source, Type objType)
        {
            var entityObjs = Array.CreateInstance(objType, source.Length);
            for (var i = 0; i < entityObjs.Length; i++)
            {
                entityObjs.SetValue(Convert.ChangeType(source.GetValue(i), objType), i);
            }
            return entityObjs;
        }

        /// <summary>
        /// 转化数组类型 
        /// </summary>
        public static TResult[] ChangeBaseType<TSource, TResult>(IEnumerable<TSource> sources)
        {
            var rets = new TResult[sources.Count()];
            for (int i = 0; i < rets.Length; i++)
            {
                rets[i] = (TResult)Convert.ChangeType(sources.ElementAt(i), typeof(TResult));
            }
            return rets;
        }

        /// <summary>
        /// 转化数组类型 
        /// </summary>
        public static TResult[] ChangeObjType<TSource, TResult>(IEnumerable<TSource> sources)
            where TSource : class
            where TResult : class
        {
            var rets = new TResult[sources.Count()];
            for (int i = 0; i < rets.Length; i++)
            {
                rets[i] = sources.ElementAt(i) as TResult;
            }
            return rets;
        }

        /// <summary>
        /// IEnumerable统一改变数组枚举值
        /// </summary>
        public static IEnumerable<TTarget> ChangeValue<TTarget>(IEnumerable arr, Func<object, TTarget> func)
        {
            List<TTarget> tarList = new List<TTarget>();
            var enumTor = arr.GetEnumerator();
            while (enumTor.MoveNext())
            {
                tarList.Add(func(enumTor.Current));
            }
            return tarList;
        }

        /// <summary>
        /// 统一改变数组枚举值
        /// </summary>
        public static IEnumerable<TTarget> ChangeValue<TSource, TTarget>(IEnumerable<TSource> arr, Func<TSource, TTarget> func)
        {
            List<TTarget> tarList = new List<TTarget>();
            var enumTor = arr.GetEnumerator();
            while (enumTor.MoveNext())
            {
                tarList.Add(func(enumTor.Current));
            }
            return tarList;
        }

        /// <summary>
        /// 拼接数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Combine<T>(params IEnumerable<T>[] arrs)
        {
            var arrList = new List<T>();
            foreach (var arr in arrs)
            {
                arrList.AddRange(arr);
            }
            return arrList;
        }
    }
}
