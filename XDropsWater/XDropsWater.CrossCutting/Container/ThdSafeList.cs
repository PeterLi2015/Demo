using System;
using System.Collections.Generic;
using System.Text;

namespace XDropsWater.CrossCutting.Container
{
    public class ThdSafeList<T> : List<T>
    {
        public ThdSafeList()
            : base()
        {

        }

        public ThdSafeList(int capacity)
            : base(capacity)
        {

        }

        public ThdSafeList(IEnumerable<T> collection)
            : base(collection)
        {

        }

        public new void Add(T item)
        {
            lock (this)
            {
                base.Add(item);
            }
        }

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(params T[] items)
        {
            lock (this)
            {
                base.AddRange(items);
            }
        }

        public new bool Remove(T item)
        {
            lock (this)
            {
                return base.Remove(item);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public new void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }

        /// <summary>
        /// 清空并返回集合元素数组
        /// </summary>
        /// <returns></returns>
        public T[] ClearOut()
        {
            T[] ts = { };
            lock (this)
            {
                ts = base.ToArray();
                base.Clear();
            }
            return ts;
        }
    }
}
