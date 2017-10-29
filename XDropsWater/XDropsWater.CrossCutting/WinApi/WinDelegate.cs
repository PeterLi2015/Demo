using System;
using System.Collections.Generic;
using System.Text;

namespace XDropsWater.CrossCutting.Win
{
    public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);
    // Delegate that is called for each child window of window. 
    public delegate bool EnumWinCallBack(IntPtr hwnd, IntPtr lParam);
}
