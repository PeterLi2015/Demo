using System;
using System.Collections.Generic;
using System.Text;

namespace XDropsWater.CrossCutting.Win
{
    /// <summary>
    /// GetWindow params
    /// </summary>
    public enum GetWindow_Cmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }


    public enum SHGFI
    {
        SHGFI_ICON = 0x100,
        SHGFI_LARGEICON = 0x0,
        SHGFI_USEFILEATTRIBUTES = 0x10
    }

    public class NAMEPIPE_CONST
    {
        #region Comments
        /// <summary>
        /// Outbound pipe access.
        /// </summary>
        #endregion
        public const uint PIPE_ACCESS_OUTBOUND = 0x00000002;
        #region Comments
        /// <summary>
        /// Duplex pipe access.
        /// </summary>
        #endregion
        public const uint PIPE_ACCESS_DUPLEX = 0x00000003;
        #region Comments
        /// <summary>
        /// Inbound pipe access.
        /// </summary>
        #endregion
        public const uint PIPE_ACCESS_INBOUND = 0x00000001;
        #region Comments
        /// <summary>
        /// Pipe blocking mode.
        /// </summary>
        #endregion
        public const uint PIPE_WAIT = 0x00000000;
        #region Comments
        /// <summary>
        /// Pipe non-blocking mode.
        /// </summary>
        #endregion
        public const uint PIPE_NOWAIT = 0x00000001;
        #region Comments
        /// <summary>
        /// Pipe read mode of type Byte.
        /// </summary>
        #endregion
        public const uint PIPE_READMODE_BYTE = 0x00000000;
        #region Comments
        /// <summary>
        /// Pipe read mode of type Message.
        /// </summary>
        #endregion
        public const uint PIPE_READMODE_MESSAGE = 0x00000002;
        #region Comments
        /// <summary>
        /// Byte pipe type.
        /// </summary>
        #endregion
        public const uint PIPE_TYPE_BYTE = 0x00000000;
        #region Comments
        /// <summary>
        /// Message pipe type.
        /// </summary>
        #endregion
        public const uint PIPE_TYPE_MESSAGE = 0x00000004;
        #region Comments
        /// <summary>
        /// Pipe client end.
        /// </summary>
        #endregion
        public const uint PIPE_CLIENT_END = 0x00000000;
        #region Comments
        /// <summary>
        /// Pipe server end.
        /// </summary>
        #endregion
        public const uint PIPE_SERVER_END = 0x00000001;
        #region Comments
        /// <summary>
        /// Unlimited server pipe instances.
        /// </summary>
        #endregion
        public const uint PIPE_UNLIMITED_INSTANCES = 255;
        #region Comments
        /// <summary>
        /// Waits indefinitely when connecting to a pipe.
        /// </summary>
        #endregion
        public const uint NMPWAIT_WAIT_FOREVER = 0xffffffff;
        #region Comments
        /// <summary>
        /// Does not wait for the named pipe.
        /// </summary>
        #endregion
        public const uint NMPWAIT_NOWAIT = 0x00000001;
        #region Comments
        /// <summary>
        /// Uses the default time-out specified in a call to the CreateNamedPipe method.
        /// </summary>
        #endregion
        public const uint NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;
        #region Comments
        /// <summary>
        /// 
        /// </summary>
        #endregion
        public const uint GENERIC_READ = (0x80000000);
        #region Comments
        /// <summary>
        /// Generic write access to the pipe.
        /// </summary>
        #endregion
        public const uint GENERIC_WRITE = (0x40000000);
        #region Comments
        /// <summary>
        /// Generic execute access to the pipe.
        /// </summary>
        #endregion
        public const uint GENERIC_EXECUTE = (0x20000000);
        #region Comments
        /// <summary>
        /// Read, write, and execute access.
        /// </summary>
        #endregion
        public const uint GENERIC_ALL = (0x10000000);
        #region Comments
        /// <summary>
        /// Create new file. Fails if the file exists.
        /// </summary>
        #endregion
        public const uint CREATE_NEW = 1;
        #region Comments
        /// <summary>
        /// Create new file. Overrides an existing file.
        /// </summary>
        #endregion
        public const uint CREATE_ALWAYS = 2;
        #region Comments
        /// <summary>
        /// Open existing file.
        /// </summary>
        #endregion
        public const uint OPEN_EXISTING = 3;
        #region Comments
        /// <summary>
        /// Open existing file. If the file does not exist, creates it.
        /// </summary>
        #endregion
        public const uint OPEN_ALWAYS = 4;
        #region Comments
        /// <summary>
        /// Opens the file and truncates it so that its size is zero bytes.
        /// </summary>
        #endregion
        public const uint TRUNCATE_EXISTING = 5;
        #region Comments
        /// <summary>
        /// Invalid operating system handle.
        /// </summary>
        #endregion
        public const int INVALID_HANDLE_VALUE = -1;
        #region Comments
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        #endregion
        public const ulong ERROR_SUCCESS = 0;
        #region Comments
        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        #endregion
        public const ulong ERROR_CANNOT_CONNECT_TO_PIPE = 2;
        #region Comments
        /// <summary>
        /// All pipe instances are busy.
        /// </summary>
        #endregion
        public const ulong ERROR_PIPE_BUSY = 231;
        #region Comments
        /// <summary>
        /// The pipe is being closed.
        /// </summary>
        #endregion
        public const ulong ERROR_NO_DATA = 232;
        #region Comments
        /// <summary>
        /// No process is on the other end of the pipe.
        /// </summary>
        #endregion
        public const ulong ERROR_PIPE_NOT_CONNECTED = 233;
        #region Comments
        /// <summary>
        /// More data is available.
        /// </summary>
        #endregion
        public const ulong ERROR_MORE_DATA = 234;
        #region Comments
        /// <summary>
        /// There is a process on other end of the pipe.
        /// </summary>
        #endregion
        public const ulong ERROR_PIPE_CONNECTED = 535;
        #region Comments
        /// <summary>
        /// Waiting for a process to open the other end of the pipe.
        /// </summary>
        #endregion
        public const ulong ERROR_PIPE_LISTENING = 536;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }

    #region SetWindowPosFlags
    [Flags]
    public enum SetWindowPosFlags : uint
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
        /// </summary>
        SWP_ASYNCWINDOWPOS = 0x4000,

        /// <summary>
        ///     Prevents generation of the WM_SYNCPAINT message.
        /// </summary>
        SWP_DEFERERASE = 0x2000,

        /// <summary>
        ///     Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        SWP_DRAWFRAME = 0x0020,

        /// <summary>
        ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary>
        ///     Hides the window.
        /// </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary>
        ///     Retains the current position (ignores X and Y parameters).
        /// </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>
        ///     Does not change the owner window's position in the Z order.
        /// </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary>
        ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        /// </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        ///     Same as the SWP_NOOWNERZORDER flag.
        /// </summary>
        SWP_NOREPOSITION = 0x0200,

        /// <summary>
        ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary>
        ///     Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>
        ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
        /// </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>
        ///     Displays the window.
        /// </summary>
        SWP_SHOWWINDOW = 0x0040,

        // ReSharper restore InconsistentNaming
    }
    #endregion

    #region SpecialWindowHandles
    /// <summary>
    ///     Special window handles
    /// </summary>
    public enum SpecialWindowHandles
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        HWND_TOP = 0,
        /// <summary>
        ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_BOTTOM = 1,
        /// <summary>
        ///     Places the window at the top of the Z order.
        /// </summary>
        HWND_TOPMOST = -1,
        /// <summary>
        ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_NOTOPMOST = -2
        // ReSharper restore InconsistentNaming
    }
    #endregion

    #region WindowShowStyle
    public enum WindowShowStyle : uint
    {
        /// <summary>Hides the window and activates another window.</summary>
        /// <remarks>See SW_HIDE</remarks>
        Hide = 0,
        /// <summary>Activates and displays a window. If the window is minimized 
        /// or maximized, the system restores it to its original size and 
        /// position. An application should specify this flag when displaying 
        /// the window for the first time.</summary>
        /// <remarks>See SW_SHOWNORMAL</remarks>
        ShowNormal = 1,
        /// <summary>Activates the window and displays it as a minimized window.</summary>
        /// <remarks>See SW_SHOWMINIMIZED</remarks>
        ShowMinimized = 2,
        /// <summary>Activates the window and displays it as a maximized window.</summary>
        /// <remarks>See SW_SHOWMAXIMIZED</remarks>
        ShowMaximized = 3,
        /// <summary>Maximizes the specified window.</summary>
        /// <remarks>See SW_MAXIMIZE</remarks>
        Maximize = 3,
        /// <summary>Displays a window in its most recent size and position. 
        /// This value is similar to "ShowNormal", except the window is not 
        /// actived.</summary>
        /// <remarks>See SW_SHOWNOACTIVATE</remarks>
        ShowNormalNoActivate = 4,
        /// <summary>Activates the window and displays it in its current size 
        /// and position.</summary>
        /// <remarks>See SW_SHOW</remarks>
        Show = 5,
        /// <summary>Minimizes the specified window and activates the next 
        /// top-level window in the Z order.</summary>
        /// <remarks>See SW_MINIMIZE</remarks>
        Minimize = 6,
        /// <summary>Displays the window as a minimized window. This value is 
        /// similar to "ShowMinimized", except the window is not activated.</summary>
        /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
        ShowMinNoActivate = 7,
        /// <summary>Displays the window in its current size and position. This 
        /// value is similar to "Show", except the window is not activated.</summary>
        /// <remarks>See SW_SHOWNA</remarks>
        ShowNoActivate = 8,
        /// <summary>Activates and displays the window. If the window is 
        /// minimized or maximized, the system restores it to its original size 
        /// and position. An application should specify this flag when restoring 
        /// a minimized window.</summary>
        /// <remarks>See SW_RESTORE</remarks>
        Restore = 9,
        /// <summary>Sets the show state based on the SW_ value specified in the 
        /// STARTUPINFO structure passed to the CreateProcess function by the 
        /// program that started the application.</summary>
        /// <remarks>See SW_SHOWDEFAULT</remarks>
        ShowDefault = 10,
        /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
        /// that owns the window is hung. This flag should only be used when 
        /// minimizing windows from a different thread.</summary>
        /// <remarks>See SW_FORCEMINIMIZE</remarks>
        ForceMinimized = 11
    }
    #endregion

}
