using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XDropsWater.CrossCutting.Hardware
{
    public static class HardwareUtil
    {
        /// <summary>
        /// 获得http客户端ID
        /// </summary>
        /// <returns></returns>
        public static string GetHttpClientIP()
        {
            if (!DotNetUtil.IsHttpRequest()) return string.Empty;
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == string.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == string.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 获取CPU的ID
        /// </summary>
        /// <returns></returns>
        public static string GetCPUID()
        {
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_Processor"))
                {
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        return mo.Properties["ProcessorId"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取CPU的ID异常.", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取服务器网卡的MAC
        /// </summary>
        /// <returns></returns>
        public static string GetMACID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc2 = mc.GetInstances();
                foreach (ManagementObject mo in moc2)
                {
                    if ((bool)mo["IPEnabled"] == true)
                        return mo["MacAddress"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取MAC的ID异常.", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取硬盘ID
        /// </summary>
        /// <returns></returns>
        public static string GetDiskDriveID()
        {
            try
            {
                ManagementClass cimobject1 = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (ManagementObject mo in moc1)
                {
                    return mo.Properties["Model"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取的硬盘ID异常.", ex);
            }
            return string.Empty;
        }
    }
}
