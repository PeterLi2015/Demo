using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.CrossCutting.String;
using System.Threading;

namespace XDropsWater.CrossCutting.Thd
{
    /// <summary>
    /// 并发冗余重复锁，值采用hash比对
    /// </summary>
    public class ConcurentRedundancyLocker : IDisposable
    {
        /// <summary>
        /// 检查有重复时的暂停间隔 单位毫秒
        /// </summary>
        public int PAUSE_INTERVAL_WHILE_REDUNDANCY = 5;
        /// <summary>
        /// 锁容器
        /// </summary>
        private static Dictionary<string, List<object[]>> lockerContainerDic = new Dictionary<string, List<object[]>>();
        /// <summary>
        /// 当前唯一锁名称
        /// </summary>
        private string curUniqueLockerName = string.Empty;
        /// <summary>
        /// 当前唯一值集
        /// </summary>
        private object[] curUniqueVals = { };

        /// <summary>
        /// 默认锁名称
        /// </summary>
        /// <param name="checkUniqueVals"></param>
        public ConcurentRedundancyLocker(params object[] checkUniqueVals)
            : this(typeof(ConcurentRedundancyLocker).AssemblyQualifiedName, checkUniqueVals)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueLockerName">锁的唯一标识名</param>
        /// <param name="checkUniqueVals">验证唯一的值集合</param>
        public ConcurentRedundancyLocker(string uniqueLockerName, params object[] checkUniqueVals)
        {
            if (FilterUtil.IsEmptyWithTrim(ref uniqueLockerName))
            {
                throw new ArgumentNullException("uniqueLockerName", "并发冗余重复锁名称不能为空.");
            }
            if (checkUniqueVals == null)
            {
                throw new ArgumentNullException("checkUniqueVals", "待验证值集合不能为null.");
            }
            this.curUniqueVals = checkUniqueVals;
            this.curUniqueLockerName = uniqueLockerName;
            CheckUnique();
        }

        /// <summary>
        /// 检查唯一
        /// </summary>
        private void CheckUnique()
        {
            if (!lockerContainerDic.ContainsKey(this.curUniqueLockerName))
            {
                lock (lockerContainerDic)
                {
                    if (!lockerContainerDic.ContainsKey(this.curUniqueLockerName))
                    {
                        lockerContainerDic.Add(this.curUniqueLockerName, new List<object[]>());
                    }
                }
            }
            var list = lockerContainerDic[this.curUniqueLockerName];

            while (true)
            {
                var exist = false;
                lock (list)
                {
                    foreach (var vals in list)
                    {
                        //选择最小数组大小避免超出索引范围
                        var len = Math.Min(vals.Length, curUniqueVals.Length);
                        //排除空集重复
                        if (vals.Length == 0 && curUniqueVals.Length == 0)
                        {
                            exist = true;
                            break;
                        }
                        //是否数组值都相等
                        var tempExist = true;
                        for (var i = 0; i < vals.Length; i++)
                        {
                            tempExist = tempExist && (vals[i].GetHashCode() == curUniqueVals[i].GetHashCode());
                            if (!tempExist) break;
                        }
                        if (tempExist)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        list.Add(this.curUniqueVals);
                    }
                }
                if (exist)
                {
                    Thread.Sleep(PAUSE_INTERVAL_WHILE_REDUNDANCY);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void UnLock()
        {
            var list = lockerContainerDic[this.curUniqueLockerName];
            lock (list)
            {
                list.Remove(this.curUniqueVals);
            }
        }

        /// <summary>
        /// 释放资源，同时解锁
        /// </summary>
        public void Dispose()
        {
            UnLock();
        }
    }
}
