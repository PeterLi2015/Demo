using System;
using System.Collections.Generic;
using System.Text;
using XDropsWater.CrossCutting.Win;

namespace XDropsWater.CrossCutting.WinApi
{
    /// <summary>
    /// 系统全局  通知一个或多个正在等待的线程已发生事件
    /// The PubManualResetEvent class is to suppliment .NET's event classes with the ability to span
    /// processes in both .NET and Win32 with the same event signaling capability.
    /// 
    /// The PubManualResetEvent class object provides the ability to wait on, pulse, set, and
    /// reset a named event. The event can be created with manual or automatic reset,
    /// and is always created with an initial state of reset(false).
    /// 
    /// The PubManualResetEvent class will not leave a handle open past any method
    /// call, so garbage collection is irrelevant.
    /// 
    /// The PubManualResetEvent class defines both instance methods and static methods.
    /// The static methods require a name, where as the instance methods contain the
    /// name in the object instance already. The static methods are also limited to
    /// the configuration of auto reset and initially not signaled.
    /// 
    /// The wait methods will wait on a single object, the named event, only. There
    /// is no multiple event support.
    ///
    /// Win32 security is a critical issue. This class does not support specifically
    /// identifying any security, other than the default process security context.
    /// This class is best suitable for processes run in the same security context,
    /// such as the desktop's interactive user account.
    /// </summary>
    public class PubManualResetEvent
    {
        private string _EventName;
        private IntPtr _Handle;
        private IntPtr _Attributes = IntPtr.Zero;
        private bool _ManualReset;
        private bool _InitialState;

        private static int INFINITE = -1;

        /// <summary>
        /// Create a PubManualResetEvent object with the name of the event, and assume auto reset with
        /// an initial state of reset.
        /// </summary>
        public PubManualResetEvent(string eventName) : this(eventName, false) { }

        /// <summary>
        /// Create a PubManualResetEvent object with the name of the event and the auto reset property,
        /// assuming an initial state of reset.
        /// </summary>
        public PubManualResetEvent(string eventName, bool manualReset)
        {
            _EventName = eventName;
            _ManualReset = manualReset;
            _InitialState = false;
        }

        /// <summary>
        /// Wait for the event to signal to a maximum period of TimeoutInSecs total seconds.
        /// Returns true if the event signaled, false if timeout occurred.
        /// </summary>
        public bool Wait(int timeoutInSecs)
        {
            _Handle = API.CreateEvent(_Attributes, _ManualReset, _InitialState, _EventName);
            int rc = API.WaitForSingleObject(_Handle, timeoutInSecs * 1000);
            API.CloseHandle(_Handle);
            return rc == 0;
        }
        /// <summary>
        /// Wait for the event to signal until the event signaled
        ///</summary>
        public bool Wait()
        {
            return Wait(INFINITE);
        }

        /// <summary>
        /// Pulse the named event, which results in a single waiting thread to exit the Wait method.
        /// </summary>
        public bool Pulse()
        {
            _Handle = API.CreateEvent(_Attributes, _ManualReset, _InitialState, _EventName);
            API.PulseEvent(_Handle);
            API.CloseHandle(_Handle);
            return _Handle != IntPtr.Zero;
        }

        /// <summary>
        /// Set the named event to a signaled state. The Wait() method will not block any
        /// thread as long as the event is in a signaled state.
        /// </summary>
        public void Set()
        {
            _Handle = API.CreateEvent(_Attributes, _ManualReset, _InitialState, _EventName);
            API.SetEvent(_Handle);
            API.CloseHandle(_Handle);
        }

        /// <summary>
        /// Reset the named event to a non signaled state. The Wait() method will block
        /// any thread that enters it as long as the event is in a non signaled state.
        /// </summary>
        public void Reset()
        {
            _Handle = API.CreateEvent(_Attributes, _ManualReset, _InitialState, _EventName);
            API.ResetEvent(_Handle);
            API.CloseHandle(_Handle);
        }

        /// <summary>
        /// Wait for the event with the given name to signal to a maximum period of TimeoutInSecs total seconds.
        /// Returns true if the event signaled, false if timeout occurred.
        /// </summary>
        public static bool Wait(int timeoutInSecs, string name)
        {
            return (new PubManualResetEvent(name)).Wait(timeoutInSecs);
        }

        public static bool Wait(string name)
        {
            return (new PubManualResetEvent(name)).Wait();
        }

        /// <summary>
        /// Pulse the event with the given name, which results in a single waiting thread to exit the Wait method.
        /// </summary>
        public static bool Pulse(string name) { return (new PubManualResetEvent(name)).Pulse(); }

        /// <summary>
        /// Set the event with the given name to a signaled state. The Wait() method will not block
        /// any threads as long as the event is in a signaled state.
        /// </summary>
        public static void Set(string name) { (new PubManualResetEvent(name)).Set(); }

        /// <summary>
        /// Reset the event with the given name to a non signaled state. The Wait() method will block
        /// any thread that enters it as long as the event is in a non signaled state.
        /// </summary>
        public static void Reset(string name) { (new PubManualResetEvent(name)).Reset(); }
    }
}
