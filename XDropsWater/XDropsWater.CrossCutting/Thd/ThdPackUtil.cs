using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;

namespace XDropsWater.CrossCutting.Thd
{
    public delegate void PausePointHandler();
    public delegate void ThreadEventHandler(object sender, ThreadEventArgs args);
    public delegate void AllStopEventHandler(object sender, object args);

    public class ThdPackUtil
    {
        /// <summary>
        /// 线程安全读取数据
        /// </summary>
        public static T ThreadSafeRead<T>(T t, object lockObject)
        {
            lock (lockObject)
            {
                return t;
            }
        }

        /// <summary>
        /// 每个子线程启动时的事件
        /// </summary>
        public event ThreadEventHandler Started;
        /// <summary>
        /// 所有子线停止事件
        /// </summary>
        public event AllStopEventHandler AllStopped;
        /// <summary>
        /// 每个子线程停止事件
        /// </summary>
        public event ThreadEventHandler Stopped;

        /// <summary>
        /// 监控任务线程是否完成
        /// </summary>
        private Thread threadMonitor;
        /// <summary>
        /// 用于暂停继续线程设置断点的字段
        /// </summary>
        private ManualResetEvent manualResetEvent;
        /// <summary>
        /// 保存执行线程的数组
        /// </summary>
        private Thread[] threadArr;
        /// <summary>
        /// 加锁对象
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// 线程简单封装类
        /// </summary>
        /// <param name="num">线程数</param>
        public ThdPackUtil(int count)
        {
            if (count < 1)
                throw new Exception("线程数不能少于一个.");
            manualResetEvent = new ManualResetEvent(true);
            //实例化子线程
            threadArr = new Thread[count];
            waitThdStopEvents = new AutoResetEvent[count];
            for (int i = 0; i < threadArr.Length; i++)
            {
                int index = i;
                threadArr[i] = new Thread(new ParameterizedThreadStart(delegate(object args)
                {
                    ThreadEventArgs thdArgs = new ThreadEventArgs();
                    thdArgs.Current = Thread.CurrentThread;
                    thdArgs.DataArg = args;
                    thdArgs.ExecPausePoint = new PausePointHandler(SetPausePoint);
                    thdArgs.Index = index;

                    //线程加强效果，防止快速结束
                    Thread.Sleep(100);
                    try
                    {
                        if (Started != null) { Started(this, thdArgs); }
                    }
                    finally
                    {
                        if (Stopped != null) { Stopped(this, thdArgs); }
                        waitThdStopEvents[thdArgs.Index].Set();
                        Thread.CurrentThread.Abort();
                    }
                }));
                threadArr[i].IsBackground = true;
                waitThdStopEvents[i] = new AutoResetEvent(false);
            }
            threadMonitor = new Thread(new ThreadStart(GuardThreads));
        }

        /// <summary>
        /// 开始运行线程
        /// </summary>
        /// <param name="args">子线程运行参数</param>
        public void Start(object args)
        {
            Start(args, false);
        }

        /// <summary>
        /// 开始运行线程 并使主线程挂起
        /// </summary>
        public void Start(object args, bool join)
        {
            if (threadMonitor.ThreadState != ThreadState.Unstarted) return;
            this.argsData = args;
            threadMonitor.Start();
            while (true)
            {
                if (threadMonitor.IsAlive)
                {
                    break;
                }
                Thread.Sleep(10);
            }
            if (join)
            {
                threadMonitor.Join();
            }
        }

        /// <summary>
        /// 任务线程设置断点的静态方法
        /// </summary>
        public void SetPausePoint()
        {
            manualResetEvent.WaitOne();
        }

        /// <summary>
        /// 暂停线程
        /// </summary>
        public void Pause()
        {
            if (!threadMonitor.IsAlive) return;
            manualResetEvent.Reset();
        }
        /// <summary>
        /// 暂停后继续线程的方法
        /// </summary>
        public void Resume()
        {
            manualResetEvent.Set();
        }

        /// <summary>
        /// 停止运行线程
        /// </summary>
        private void AsycEndThread()
        {
            Pause();
            Thread.Sleep(500);
            for (int i = 0; i < threadArr.Length; i++)
            {
                Thread thd = threadArr[i];
                lock (thd)
                {
                    if (thd.IsAlive)
                    {
                        if (thd.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            thd.Interrupt();
                        }
                        else
                        {
                            thd.Abort();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 外部调用 终止线程
        /// </summary>
        public void Stop()
        {
            if (!threadMonitor.IsAlive) return;
            AsycEndThread();
            //等待线程结束
            threadMonitor.Join();
        }
        /// <summary>
        /// 外部调用 终止线程 提供超时时间
        /// </summary>
        public bool Stop(int miliSecond)
        {
            AsycEndThread();
            //等待线程结束
            return threadMonitor.Join(miliSecond);
        }


        private object argsData = null;
        /// <summary>
        /// 子线程运行参数
        /// </summary>
        public object ArgsData
        {
            get { return argsData; }
            set { argsData = value; }
        }

        /// <summary>
        /// 子线程优先权
        /// </summary>
        public ThreadPriority Priority
        {
            set
            {
                for (int i = 0; i < threadArr.Length; i++)
                {
                    threadArr[i].Priority = value;
                }
            }
        }

        /// <summary>
        /// 赋值是否为后台线程
        /// </summary>
        public bool IsBackground
        {
            set
            {
                for (int i = 0; i < threadArr.Length; i++)
                {
                    threadArr[i].IsBackground = value;
                }
            }
        }

        /// <summary>
        /// 子线程数
        /// </summary>
        public int Count
        {
            get { return threadArr.Length; }
        }

        private AutoResetEvent[] waitThdStopEvents = null;
        /// <summary>
        ///  监控线程是否完成
        /// </summary>
        private void GuardThreads()
        {
            for (int i = 0; i < Count; i++)
            {
                threadArr[i].Start(ArgsData);
            }
            WaitHandle.WaitAll(waitThdStopEvents);

            if (AllStopped != null)
            {
                AllStopped(this, argsData);
            }
        }
    }
    /// <summary>
    /// 进程运行参数
    /// </summary>
    public class ThreadEventArgs : EventArgs
    {
        /// <summary>
        /// 进程运行扩展参数
        /// </summary>
        public object DataArg = null;
        /// <summary>
        /// 当前线程
        /// </summary>
        public Thread Current = null;
        /// <summary>
        /// 设置线程暂停点或断点
        /// </summary>
        public PausePointHandler ExecPausePoint = null;
        /// <summary>
        /// 当前线程在线程包里的线程位置
        /// </summary>
        public int Index = 0;
    }
}
