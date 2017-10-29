using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace XDropsWater.CrossCutting.Win
{
    //声明一个Rectangle的封送类型   
    [StructLayout(LayoutKind.Sequential)]
    public struct WinRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    //声明一个Point的封送类型   
    [StructLayout(LayoutKind.Sequential)]
    public class WinPoint
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// Sets the information that the SCROLLINFO structure maintains about a scroll bar. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SCROLLINFO
    {
        public int cbSize;
        public int fMask;
        public int nMin;
        public int nMax;
        public int nPage;
        public int nPos;
        public int nTrackPos;
    }

    /// <summary>
    /// Security Descriptor structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_DESCRIPTOR
    {
        private byte Revision;
        private byte Sbz1;
        private ushort Control;
        private IntPtr Owner;
        private IntPtr Group;
        private IntPtr Sacl;
        private IntPtr Dacl;
    }

    /// <summary>
    /// Security Attributes structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
    #region Comments
    /// <summary>
    /// This class is used as a dummy parameter only.
    /// </summary>
    #endregion
    [StructLayout(LayoutKind.Sequential)]
    public class Overlapped
    {
    }
    #region Comments
    /// <summary>
    /// This class is used as a dummy parameter only.
    /// </summary>
    #endregion
    [StructLayout(LayoutKind.Sequential)]
    public class SecurityAttributes
    {
    }

    /// <summary>
    /// Contains information about a file object. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    /// <summary>
    /// The NETRESOURCE structure contains information about a network resource.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }
}
