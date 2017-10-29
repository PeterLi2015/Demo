using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace XDropsWater.CrossCutting.Container
{
    /// <summary>
    /// Session模式功能适配器
    /// </summary>
    public abstract class SessionAdapter
    {
        /// <summary>
        /// 当前会话SessionID标识
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// 关联指定线程与session 必须在顶级父线程上调用此函数
        /// </summary>
        /// <param name="childThd"></param>
        public abstract void AttachThread(Thread childThd);

        /// <summary>
        /// 向会话状态集合添加一个新项。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void Add(string name, object value);
        /// <summary>
        /// 按名称获取或设置会话值。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract object this[string name] { get; set; }
        /// <summary>
        /// 删除会话状态集合中的项。
        /// </summary>
        /// <param name="name"></param>
        public abstract void Remove(string name);

        /// <summary>
        /// 释放当前会话状态
        /// </summary>
        public abstract void Abandon();

        /// <summary>
        /// 释放指定ID的会话状态
        /// </summary>
        public abstract void Abandon(string id);

        /// <summary>
        ///获取并设置在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。
        /// </summary>
        public virtual int Timeout { get; set; }
    }

    /// <summary>
    /// 父线程隔离Session适配器
    /// </summary>
    public class ThreadSessionAdapter : SessionAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ThreadSessionAdapter CurObj = new ThreadSessionAdapter();

        private Dictionary<Thread, Thread> dicThdRelDic = new Dictionary<Thread, Thread>();
        private Dictionary<Thread, ThreadSession> sessionWrapDic = new Dictionary<Thread, ThreadSession>();

        /// <summary>
        /// 
        /// </summary>
        private ThreadSessionAdapter()
        {
            this.Timeout = 20;
            StartClearThd();
        }
        /// <summary>
        /// 当前会话SessionID标识
        /// </summary>
        public override string ID
        {
            get
            {
                Thread parentThd = Thread.CurrentThread;
                if (dicThdRelDic.ContainsKey(Thread.CurrentThread))
                {
                    parentThd = dicThdRelDic[Thread.CurrentThread];
                }
                return parentThd.ManagedThreadId.ToString();
            }
        }

        /// <summary>
        /// 启动清理线程
        /// </summary>
        private void StartClearThd()
        {
            Thread thd = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        //清空失效的父子线程关系映射
                        Thread[] thdKeys = dicThdRelDic.Keys.ToArray<Thread>();
                        foreach (var thdKey in thdKeys)
                        {
                            if (thdKey.ThreadState == ThreadState.Aborted ||
                                thdKey.ThreadState == ThreadState.Stopped)
                            {
                                dicThdRelDic.Remove(thdKey);
                            }
                        }
                        //清空失效的session
                        thdKeys = sessionWrapDic.Keys.ToArray<Thread>();
                        foreach (var thdKey in thdKeys)
                        {
                            if (thdKey.ThreadState == ThreadState.Aborted ||
                                thdKey.ThreadState == ThreadState.Stopped)
                            {
                                sessionWrapDic.Remove(thdKey);
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        //暂停30秒
                        Thread.Sleep(30 * 1000);
                    }
                }
            });
            thd.IsBackground = true;
            thd.Priority = ThreadPriority.Lowest;
            thd.Start();
        }

        /// <summary>
        /// 关联指定线程与session
        /// </summary>
        /// <param name="childThd"></param>
        public override void AttachThread(Thread childThd)
        {
            this.AttachThread(Thread.CurrentThread, childThd);
        }

        /// <summary>
        /// 绑定父子线程关系
        /// </summary>
        /// <param name="parentThd">父线程</param>
        /// <param name="childThd">子线程</param>
        public void AttachThread(Thread parentThd, Thread childThd)
        {
            if (dicThdRelDic.ContainsKey(childThd))
            {
                dicThdRelDic.Remove(childThd);
            }
            //线程
            dicThdRelDic.Add(childThd, parentThd);
        }

        /// <summary>
        /// 获得当前线程对应的隔离session模拟对象
        /// </summary>
        private ThreadSession CurSessionWrap
        {
            get
            {
                Thread parentThd = null;
                if (dicThdRelDic.ContainsKey(Thread.CurrentThread))
                {
                    parentThd = dicThdRelDic[Thread.CurrentThread];
                }
                else
                {
                    parentThd = Thread.CurrentThread;
                    dicThdRelDic.Add(parentThd, parentThd);
                    sessionWrapDic.Add(parentThd, new ThreadSession());
                }
                return sessionWrapDic[parentThd];
            }
        }
        /// <summary>
        ///         //
        // 摘要:
        //     按名称获取或设置会话值。
        //
        // 参数:
        //   name:
        //     会话值的键名。
        //
        // 返回结果:
        //     具有指定名称的会话状态值；如果该项不存在，则为 null。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[string name]
        {
            get
            {
                var csw = CurSessionWrap;
                lock (csw)
                {
                    var sessionWrap = csw as ThreadSession;
                    if (sessionWrap == null) return null;
                    if (sessionWrap.VisitDT.AddMinutes(Timeout) < DateTime.Now)
                    {
                        Remove(name);
                        return null;
                    }
                    return sessionWrap.Session[name];
                }
            }
            set
            {
                var chd = CurSessionWrap;
                lock (chd)
                {
                    chd.Session[name] = value;
                }
            }
        }
        /// <summary>
        /// 向会话状态集合添加一个新项
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void Add(string name, object value)
        {
            var chd = CurSessionWrap;
            lock (chd)
            {
                chd.Session.Add(name, value);
            }
        }
        /// <summary>
        /// 删除会话状态集合中的项。
        /// </summary>
        /// <param name="name"></param>
        public override void Remove(string name)
        {
            var chd = CurSessionWrap;
            lock (chd)
            {
                chd.Session.Remove(name);
            }
        }
        /// <summary>
        /// 取消当前会话
        /// </summary>
        public override void Abandon()
        {
            Abandon(Thread.CurrentThread.ManagedThreadId.ToString());
        }

        /// <summary>
        /// 释放指定ID的会话状态
        /// </summary>
        public override void Abandon(string id)
        {
            //查找id对应的线程
            Thread thd = null;
            foreach (var thdTemp in sessionWrapDic.Keys)
            {
                if (thdTemp.ManagedThreadId.ToString() == id)
                {
                    thd = thdTemp;
                    break;
                }
            }
            //删除对应线程的会话
            if (thd != null)
            {
                this.sessionWrapDic.Remove(thd);
                GC.Collect();
            }
        }

        /// <summary>
        /// 线程类型session
        /// </summary>
        private class ThreadSession
        {
            public ThreadSession()
            {
                this.VisitDT = DateTime.Now;
            }
            private HybridDictionary session = new HybridDictionary();
            /// <summary>
            /// 获得当前线程对应的隔离session模拟对象
            /// </summary>
            public HybridDictionary Session
            {
                get
                {
                    this.VisitDT = DateTime.Now;
                    return this.session;
                }
            }
            /// <summary>
            /// 访问值时间
            /// </summary>
            public DateTime VisitDT { get; private set; }
        }
    }
    /// <summary>
    /// Http程序域隔离Session适配器
    /// </summary>
    public class HttpSessionAdapter : SessionAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly HttpSessionAdapter CurObj = new HttpSessionAdapter();

        /// <summary>
        /// 
        /// </summary>
        private HttpSessionAdapter()
        {

        }

        /// <summary>
        /// 待释放的SessionID集合
        /// </summary>
        private ThdSafeList<string> invalidSessionIDList = new ThdSafeList<string>();
        /// <summary>
        /// 当前会话SessionID标识
        /// </summary>
        public override string ID
        {
            get
            {
                if (CurSession == null) return string.Empty;
                return CurSession.SessionID.ToString();
            }
        }

        /// <summary>
        /// 记录线程和Session关联字典
        /// </summary>
        private Dictionary<Thread, HttpSessionState> thdSessionDic = new Dictionary<Thread, HttpSessionState>();
        /// <summary>
        /// 关联指定线程与session 必须在父线程上调用此函数
        /// </summary>
        /// <param name="childThd"></param>
        public override void AttachThread(Thread childThd)
        {
            if (thdSessionDic.ContainsKey(childThd))
            {
                this.thdSessionDic.Add(childThd, thdSessionDic[childThd]);
            }
            else
            {
                this.thdSessionDic.Add(childThd, HttpContext.Current.Session);
            }
        }

        /// <summary>
        /// 向会话状态集合添加一个新项。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void Add(string name, object value)
        {
            if (CurSession == null) return;
            CurSession.Add(name, value);
        }
        /// <summary>
        /// 按名称获取或设置会话值。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object this[string name]
        {
            get
            {
                if (CurSession == null) return null;
                return CurSession[name];
            }
            set
            {
                if (CurSession == null) return;
                CurSession[name] = value;
            }
        }
        /// <summary>
        /// 删除会话状态集合中的项。
        /// </summary>
        /// <param name="name"></param>
        public override void Remove(string name)
        {
            if (CurSession == null) return;
            CurSession.Remove(name);
        }

        /// <summary>
        /// 取消当前会话
        /// </summary>
        public override void Abandon()
        {
            if (CurSession == null) return;
            DisposeSession(CurSession);
        }

        /// <summary>
        /// 释放指定ID的会话状态
        /// </summary>
        public override void Abandon(string id)
        {
            if (!invalidSessionIDList.Contains(id))
            {
                invalidSessionIDList.Add(id);
            }
        }

        /// <summary>
        /// 释放session
        /// </summary>
        /// <param name="session"></param>
        private void DisposeSession(HttpSessionState session)
        {
            lock (invalidSessionIDList)
            {
                if (session == null) return;
                invalidSessionIDList.Remove(session.SessionID);
            }
            foreach (var key in session)
            {
                try
                {
                    var d = session[key.ToString()] as IDisposable;
                    if (d != null)
                    {
                        d.Dispose();
                    }
                }
                catch { }
            }
            session.Clear();
            session.Abandon();
            GC.Collect();
        }

        /// <summary>
        /// 当前会话对象
        /// </summary>
        private HttpSessionState CurSession
        {
            get
            {
                HttpSessionState se = null;
                if (thdSessionDic.ContainsKey(Thread.CurrentThread))
                {
                    se = thdSessionDic[Thread.CurrentThread];
                }
                else
                {
                    if (HttpContext.Current == null) return null;
                    se = HttpContext.Current.Session;
                }

                if (se == null) return null;
                //HttpContext.Current.Session 在Application_start时候为空 所以使用timeOut临时解决
                if (se.Timeout != this.timeOut)
                {
                    se.Timeout = this.timeOut;
                }
                if (invalidSessionIDList.Contains(se.SessionID))
                {
                    DisposeSession(se);
                    return null;
                }
                return se;
            }
        }

        //HttpContext.Current.Session 在Application_start时候为空 所以使用timeOut临时解决
        private int timeOut = 20;
        /// <summary>
        /// 获取并设置在会话状态提供程序终止会话之前各请求之间所允许的时间（以分钟为单位）。
        /// </summary>
        public override int Timeout
        {
            get
            {
                if (HttpContext.Current.Session == null) return this.timeOut;
                return HttpContext.Current.Session.Timeout;
            }
            set
            {
                this.timeOut = value;
                if (HttpContext.Current.Session == null) return;
                HttpContext.Current.Session.Timeout = value;
            }
        }
    }
}