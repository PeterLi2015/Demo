using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace XDropsWater.CrossCutting.Win
{
    /// <summary>
    /// 所有Win API的.net代理在网站http://www.pinvoke.net
    /// </summary>
    public static class API
    {
        #region GDI32
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateDC(string driverName, string deviceName, string output, IntPtr lpinitData);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteDC(IntPtr DC);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePolygonRgn(Point[] lppt, int cPoints, int fnPolyFillMode);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        #endregion

        #region Kernel32
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool GenerateConsoleCtrlEvent(int dwCtrlEvent, int dwProcessSessionID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ResetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool PulseEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern Int32 WaitForSingleObject(IntPtr handle, Int32 milliseconds);

        /// <summary>
        /// Creates an instance of a named pipe and returns a handle for 
        /// subsequent pipe operations.
        /// </summary>
        /// <param name="lpName">Pointer to the null-terminated string that 
        /// uniquely identifies the pipe.</param>
        /// <param name="dwOpenMode">Pipe access mode, the overlapped mode, 
        /// the write-through mode, and the security access mode of the pipe handle.</param>
        /// <param name="dwPipeMode">Type, read, and wait modes of the pipe handle.</param>
        /// <param name="nMaxInstances">Maximum number of instances that can be 
        /// created for this pipe.</param>
        /// <param name="nOutBufferSize">Number of bytes to reserve for the output buffer.</param>
        /// <param name="nInBufferSize">Number of bytes to reserve for the input buffer.</param>
        /// <param name="nDefaultTimeOut">Default time-out value, in milliseconds.</param>
        /// <param name="pipeSecurityDescriptor">Pointer to a 
        /// <see cref="AppModule.NamedPipes.SECURITY_ATTRIBUTES">SECURITY_ATTRIBUTES</see> 
        /// object that specifies a security descriptor for the new named pipe.</param>
        /// <returns>If the function succeeds, the return value is a handle 
        /// to the server end of a named pipe instance.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateNamedPipe(
            string lpName,									// pipe name
            uint dwOpenMode,								// pipe open mode
            uint dwPipeMode,								// pipe-specific modes
            uint nMaxInstances,							// maximum number of instances
            uint nOutBufferSize,						// output buffer size
            uint nInBufferSize,							// input buffer size
            uint nDefaultTimeOut,						// time-out interval
            IntPtr pipeSecurityDescriptor		// SD
            );
        #region Comments
        /// <summary>
        /// Enables a named pipe server process to wait for a client 
        /// process to connect to an instance of a named pipe.
        /// </summary>
        /// <param name="hHandle">Handle to the server end of a named pipe instance.</param>
        /// <param name="lpOverlapped">Pointer to an 
        /// <see cref="AppModule.NamedPipes.Overlapped">Overlapped</see> object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ConnectNamedPipe(
            IntPtr hHandle,									// handle to named pipe
            Overlapped lpOverlapped					// overlapped structure
            );
        #region Comments
        /// <summary>
        /// Connects to a message-type pipe (and waits if an instance of the 
        /// pipe is not available), writes to and reads from the pipe, and then closes the pipe.
        /// </summary>
        /// <param name="lpNamedPipeName">Pointer to a null-terminated string 
        /// specifying the pipe name.</param>
        /// <param name="lpInBuffer">Pointer to the buffer containing the data written 
        /// to the pipe.</param>
        /// <param name="nInBufferSize">Size of the write buffer, in bytes.</param>
        /// <param name="lpOutBuffer">Pointer to the buffer that receives the data 
        /// read from the pipe.</param>
        /// <param name="nOutBufferSize">Size of the read buffer, in bytes.</param>
        /// <param name="lpBytesRead">Pointer to a variable that receives the number 
        /// of bytes read from the pipe.</param>
        /// <param name="nTimeOut">Number of milliseconds to wait for the 
        /// named pipe to be available.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CallNamedPipe(
            string lpNamedPipeName,
            byte[] lpInBuffer,
            uint nInBufferSize,
            byte[] lpOutBuffer,
            uint nOutBufferSize,
            byte[] lpBytesRead,
            int nTimeOut
            );
        #region Comments
        /// <summary>
        /// Creates or opens a file, directory, physical disk, volume, console buffer, 
        /// tape drive, communications resource, mailslot, or named pipe.
        /// </summary>
        /// <param name="lpFileName">Pointer to a null-terminated string that 
        /// specifies the name of the object to create or open.</param>
        /// <param name="dwDesiredAccess">Access to the object (reading, writing, or both).</param>
        /// <param name="dwShareMode">Sharing mode of the object (reading, writing, both, or neither).</param>
        /// <param name="attr">Pointer to a 
        /// <see cref="AppModule.NamedPipes.SecurityAttributes">SecurityAttributes</see> 
        /// object that determines whether the returned handle can be inherited 
        /// by child processes.</param>
        /// <param name="dwCreationDisposition">Action to take on files that exist, 
        /// and which action to take when files do not exist.</param>
        /// <param name="dwFlagsAndAttributes">File attributes and flags.</param>
        /// <param name="hTemplateFile">Handle to a template file, with the GENERIC_READ access right.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified file.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,						  // file name
            uint dwDesiredAccess,					  // access mode
            uint dwShareMode,								// share mode
            SecurityAttributes attr,				// SD
            uint dwCreationDisposition,			// how to create
            uint dwFlagsAndAttributes,			// file attributes
            uint hTemplateFile);					  // handle to template file

        /// <summary>
        /// Reads data from a file, starting at the position indicated by the file pointer.
        /// </summary>
        /// <param name="hHandle">Handle to the file to be read.</param>
        /// <param name="lpBuffer">Pointer to the buffer that receives the data read from the file.</param>
        /// <param name="nNumberOfBytesToRead">Number of bytes to be read from the file.</param>
        /// <param name="lpNumberOfBytesRead">Pointer to the variable that receives the number of bytes read.</param>
        /// <param name="lpOverlapped">Pointer to an 
        /// <see cref="AppModule.NamedPipes.Overlapped">Overlapped</see> object.</param>
        /// <returns>The ReadFile function returns when one of the following 
        /// conditions is met: a write operation completes on the write end of 
        /// the pipe, the number of bytes requested has been read, or an error occurs.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hHandle,											// handle to file
            byte[] lpBuffer,								// data buffer
            uint nNumberOfBytesToRead,			// number of bytes to read
            byte[] lpNumberOfBytesRead,			// number of bytes read
            uint lpOverlapped								// overlapped buffer
            );

        #region Comments
        /// <summary>
        /// Writes data to a file at the position specified by the file pointer.
        /// </summary>
        /// <param name="hHandle">Handle to the file.</param>
        /// <param name="lpBuffer">Pointer to the buffer containing the data to be written to the file.</param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten">Pointer to the variable that receives the number of bytes written.</param>
        /// <param name="lpOverlapped">Pointer to an 
        /// <see cref="AppModule.NamedPipes.Overlapped">Overlapped</see> object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
            IntPtr hHandle,											// handle to file
            byte[] lpBuffer,							  // data buffer
            uint nNumberOfBytesToWrite,			// number of bytes to write
            byte[] lpNumberOfBytesWritten,	// number of bytes written
            uint lpOverlapped								// overlapped buffer
            );
        #region Comments
        /// <summary>
        /// Retrieves information about a specified named pipe.
        /// </summary>
        /// <param name="hHandle">Handle to the named pipe for which information is wanted.</param>
        /// <param name="lpState">Pointer to a variable that indicates the current 
        /// state of the handle.</param>
        /// <param name="lpCurInstances">Pointer to a variable that receives the 
        /// number of current pipe instances.</param>
        /// <param name="lpMaxCollectionCount">Pointer to a variable that receives 
        /// the maximum number of bytes to be collected on the client's computer 
        /// before transmission to the server.</param>
        /// <param name="lpCollectDataTimeout">Pointer to a variable that receives 
        /// the maximum time, in milliseconds, that can pass before a remote named 
        /// pipe transfers information over the network.</param>
        /// <param name="lpUserName">Pointer to a buffer that receives the 
        /// null-terminated string containing the user name string associated 
        /// with the client application. </param>
        /// <param name="nMaxUserNameSize">Size of the buffer specified by the 
        /// lpUserName parameter.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNamedPipeHandleState(
            IntPtr hHandle,
            IntPtr lpState,
            ref uint lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            IntPtr lpUserName,
            IntPtr nMaxUserNameSize
            );
        #region Comments
        /// <summary>
        /// Cancels all pending input and output (I/O) operations that were 
        /// issued by the calling thread for the specified file handle.
        /// </summary>
        /// <param name="hHandle">Handle to a file.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CancelIo(
            IntPtr hHandle
            );
        #region Comments
        /// <summary>
        /// Waits until either a time-out interval elapses or an instance 
        /// of the specified named pipe is available for connection.
        /// </summary>
        /// <param name="name">Pointer to a null-terminated string that specifies 
        /// the name of the named pipe.</param>
        /// <param name="timeout">Number of milliseconds that the function will 
        /// wait for an instance of the named pipe to be available.</param>
        /// <returns>If an instance of the pipe is available before the 
        /// time-out interval elapses, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WaitNamedPipe(
            string name,
            int timeout);
        #region Comments
        /// <summary>
        /// Sets the read mode and the blocking mode of the specified named pipe.
        /// </summary>
        /// <remarks>
        /// If the specified handle is to the client end of a named pipe and if 
        /// the named pipe server process is on a remote computer, the function 
        /// can also be used to control local buffering.
        /// </remarks>
        /// <param name="hHandle">Handle to the named pipe instance.</param>
        /// <param name="mode">Pointer to a variable that supplies the new mode.</param>
        /// <param name="cc">Pointer to a variable that specifies the maximum 
        /// number of bytes collected on the client computer before 
        /// transmission to the server.</param>
        /// <param name="cd">Pointer to a variable that specifies the 
        /// maximum time, in milliseconds, that can pass before a remote 
        /// named pipe transfers information over the network.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetNamedPipeHandleState(
            IntPtr hHandle,
            ref uint mode,
            IntPtr cc,
            IntPtr cd);

        #region Comments
        /// <summary>
        /// Flushes the buffers of the specified file and causes all buffered data to be written to the file.
        /// </summary>
        /// <param name="hHandle">Handle to an open file.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FlushFileBuffers(
            IntPtr hHandle);
        #region Comments
        /// <summary>
        /// Disconnects the server end of a named pipe instance from a client process.
        /// </summary>
        /// <param name="hHandle">Handle to an instance of a named pipe.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        #endregion
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DisconnectNamedPipe(
            IntPtr hHandle);

        /// <summary>
        /// Retrieves the calling thread's last-error code value.
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code value.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetLastError();
        #endregion

        #region USER32
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Calls EnumWinCallBack for each child window of hWndParent (i.e. the ListView).
        [DllImport("user32.Dll", SetLastError = true)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWinCallBack lpEnumFunc, IntPtr lParam);

        // Gets the bounding rectangle of the specified window (ListView header bar). 
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out WinRect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO ScrollInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetScrollInfo(IntPtr hWnd, int fnBar, SCROLLINFO lpsi, bool fRedraw);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, SpecialWindowHandles hWndInsertAfter,
            int x, int y, int Width, int Height, SetWindowPosFlags flags);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr GetParent(IntPtr hWnd);
        //[DllImport("User32.Dll")]
        //public static extern int GetDlgCtrlID(IntPtr hWndCtl);
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //public static extern int MapWindowPoints(IntPtr hWnd, IntPtr hWndTo, ref POINT pt, int cPoints);
        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);
        //[DllImport("User32.Dll")]
        //public static extern void GetWindowText(IntPtr hWnd, StringBuilder param, int length);
        [DllImport("User32.Dll")]
        public static extern void GetClassName(IntPtr hWnd, StringBuilder param, int length);
        //[DllImport("user32.Dll")]
        //public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsCallBack lpEnumFunc, int lParam);
        //[DllImport("user32.Dll")]
        //public static extern bool EnumWindows(EnumWindowsCallBack lpEnumFunc, int lParam);
        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern bool ReleaseCapture();
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SetCapture(IntPtr hWnd);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr ChildWindowFromPointEx(IntPtr hParent, POINT pt, ChildFromPointFlags flags);
        [DllImport("user32.dll", EntryPoint = "FindWindowExA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        //[DllImport("user32.dll")]
        //public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, uint globalMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder param);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, char[] chars);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, SetWindowPosFlags flags);
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
        //[DllImport("user32.dll")]
        //public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);
        //[DllImport("user32.dll")]
        //public static extern bool GetClientRect(IntPtr hwnd, ref RECT rect);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string regMsg);
        #endregion

        #region Advapi32
        /// <summary>
        /// Sets the security descriptor attributes
        /// </summary>
        /// <param name="sd">Reference to a SECURITY_DESCRIPTOR structure.</param>
        /// <param name="bDaclPresent"></param>
        /// <param name="Dacl"></param>
        /// <param name="bDaclDefaulted"></param>
        /// <returns></returns>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(ref SECURITY_DESCRIPTOR sd, bool bDaclPresent, IntPtr Dacl, bool bDaclDefaulted);
        /// <summary>
        /// Initializes a SECURITY_DESCRIPTOR structure.
        /// </summary>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(out SECURITY_DESCRIPTOR sd, int dwRevision);
        #endregion

        #region Shell32
        /// <summary>   
        /// 返回系统设置的图标   
        /// </summary>   
        /// <param name="pszPath">文件路径 如果为""  返回文件夹的</param>   
        /// <param name="dwFileAttributes">0</param>   
        /// <param name="psfi">结构体</param>   
        /// <param name="cbSizeFileInfo">结构体大小</param>   
        /// <param name="uFlags">枚举类型</param>   
        /// <returns>-1失败</returns>   
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref   SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        #endregion

        #region Mpr
        /// <summary>
        /// The WNetAddConnection2 function makes a connection to a network resource and can redirect a local device to the network resource.
        /// </summary>
        [DllImport("Mpr.dll", SetLastError = true)]
        public static extern uint WNetAddConnection2(NetResource lpNetResource, string lpPassword, string lpUsername, uint dwFlags);
        /// <summary>
        /// The WNetCancelConnection2 function cancels an existing network connection. You can also call the function to remove remembered network connections that are not currently connected.
        /// </summary>
        [DllImport("Mpr.dll", EntryPoint = "WNetCancelConnection2", SetLastError = true)]
        public static extern uint WNetCancelConnection2(string lpName, uint dwFlags, bool fForce);
        #endregion

        /// <summary>
        /// 返回高位
        /// </summary>
        /// <param name="ptr">句柄数值</param>
        public static short HiWord(IntPtr ptr)
        {
            return (short)((int)ptr >> 16);
        }
    }
}
