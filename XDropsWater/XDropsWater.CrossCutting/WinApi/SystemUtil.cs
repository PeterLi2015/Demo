using System;
using System.Collections.Generic;
using System.Text;

namespace XDropsWater.CrossCutting.Win
{
    public static class SystemContainer
    {
        /// <summary>
        /// 判断是否vista以上版本
        /// </summary>
        public static bool IsWindowsVistaOrNewer()
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6);
        }
    }
}
