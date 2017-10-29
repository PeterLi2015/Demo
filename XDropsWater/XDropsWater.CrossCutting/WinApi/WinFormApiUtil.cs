using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace XDropsWater.CrossCutting.Win
{
    public class WinFormApiContainer
    {
        /// <summary>
        /// 获得进程主窗体句柄
        /// </summary>
        public static IntPtr[] GetMainWindowHandlesByPID(int processId)
        {
            List<IntPtr> mainWindowHandleList = new List<IntPtr>();
            EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(
                delegate(IntPtr handle, IntPtr extraParameter)
                {
                    int num;
                    API.GetWindowThreadProcessId(handle, out num);
                    if ((num == processId) && API.GetParent(handle) == IntPtr.Zero)
                    {
                        mainWindowHandleList.Add(handle);
                        return true;
                    }
                    return true;
                });
            API.EnumWindows(callback, IntPtr.Zero);
            return mainWindowHandleList.ToArray();
        }

        /// <summary>
        /// 设置窗体为无边框风格
        /// </summary>
        /// <param name="hWnd"></param>
        public static void SetWindowNoBorder(IntPtr hWnd)
        {
            const int GWL_STYLE = -16;
            const long WS_CAPTION = 0x00C00000L;
            const long WS_CAPTION_2 = 0X00C0000L;

            long oldstyle = API.GetWindowLong(hWnd, GWL_STYLE);

            API.SetWindowLong(hWnd, GWL_STYLE, (int)(oldstyle & (~(WS_CAPTION | WS_CAPTION_2))));
        }

        /// <summary>
        /// 获取父控件中指定类名的子控件句柄
        /// </summary>
        /// <param name="parent">父控件句柄</param>
        /// <param name="className">窗体类名</param>
        /// <param name="recursion">是否递归</param>
        /// <returns>匹配的句柄集合</returns>
        public static IntPtr[] GetChildWindows(IntPtr parent, string className, bool recursion)
        {
            List<IntPtr> result = new List<IntPtr>();
            EnumWinCallBack callBack = new EnumWinCallBack(delegate(IntPtr hwnd, IntPtr lParam)
                {
                    StringBuilder sb = new StringBuilder(200);
                    API.GetClassName(hwnd, sb, sb.Capacity);
                    if (recursion)
                    {
                        IntPtr[] ipTempList = GetChildWindows(hwnd, className, recursion);
                        result.AddRange(ipTempList);
                    }
                    if (sb.ToString() != className) return false;
                    result.Add(hwnd);
                    return true;
                });
            API.EnumChildWindows(parent, callBack, IntPtr.Zero);
            return result.ToArray();
        }
    }
}
