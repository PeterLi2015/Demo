using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace XDropsWater.CrossCutting.Thd
{
    /// <summary>
    /// 线程的扩张方法
    /// </summary>
    public class ThdUtil
    {
        /// <summary>
        /// 全局共有锁对象
        /// </summary>
        public static readonly object LOCK = new object();

        /// <summary>
        /// 在指定时间内杀死当前进程
        /// </summary>
        /// <param name="seconds">间隔时间，单位秒</param>
        public static void AsycKillTheProcessAfterSec(int seconds)
        {
            AsycKillTheProcessAfterSec(seconds, Process.GetCurrentProcess());
        }
        /// <summary>
        /// 在指定时间内杀死进程
        /// </summary>
        /// <param name="seconds">间隔时间，单位秒</param>
        /// <param name="p">指定进程</param>
        public static void AsycKillTheProcessAfterSec(int seconds, Process p)
        {
            if (p == null) return;
            new Thread(new ThreadStart(delegate()
            {
                try
                {
                    Thread.Sleep(seconds * 1000);
                    p.Kill();
                }
                catch { }
            })).Start();
        }
    }
}
