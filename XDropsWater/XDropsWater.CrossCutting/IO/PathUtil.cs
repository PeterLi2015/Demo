using XDropsWater.CrossCutting.String;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace XDropsWater.CrossCutting.IO
{
    /// <summary>
    /// 路径Path扩展
    /// </summary>
    public class PathUtil
    {
        /// <summary>
        /// 转化成http格式路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ConvertoHttpPath(string path)
        {
            return Regex.Replace(path, @"http\:\\", "http://", RegexOptions.IgnoreCase).Replace('\\', '/');
        }

        /// <summary>
        /// 获取网址目录
        /// </summary>
        public static string GetHttpDirPath(string path)
        {
            return ConvertoHttpPath(Path.GetDirectoryName(path).Trim());
        }
        /// <summary>
        /// 获取网址地址
        /// </summary>
        public static string GetHttpFilePath(string path)
        {
            return ConvertoHttpPath(Path.GetFullPath(path).Trim());
        }

        /// <summary>
        /// 拼接网址目录
        /// </summary>
        public static string CombineHttpPath(params string[] paths)
        {
            string totalPath = string.Empty;
            foreach (string path in paths)
            {
                if (string.IsNullOrEmpty(totalPath))
                {
                    totalPath = path;
                }
                else
                {
                    totalPath = Path.Combine(totalPath, path);
                }
            }
            return ConvertoHttpPath(totalPath.Trim());
        }

        /// <summary>
        /// 比较两个路径是否相同
        /// </summary>
        /// <param name="pathA">路径a</param>
        /// <param name="pathB">路径b</param>
        /// <returns></returns>
        public static bool EqualPath(string pathA, string pathB)
        {
            return Path.GetFullPath(pathA).ToLower() == Path.GetFullPath(pathB).ToLower();
        }

        /// <summary>
        /// 根据物理路径获得Web相对路径
        /// </summary>
        /// <param name="physicalPath"></param>
        /// <returns></returns>
        public static string GetWebRelPathByPhysicalPath(string physicalPath)
        {
            string appPath = Path.GetFullPath(HttpContext.Current.Server.MapPath("/"));
            physicalPath = Path.GetFullPath(physicalPath);
            if (physicalPath.IndexOf(appPath) < 0)
            {
                throw new IOException("物理路径不在当前web应用程序根目录下.");
            }
            physicalPath = physicalPath.Replace(appPath, string.Empty);
            return string.Format("/{0}", physicalPath.Replace('\\', '/').TrimStart('/'));
        }

        /// <summary>
        /// 根据web相对路径获取物理全路径
        /// </summary>
        /// <param name="webRelPath"></param>
        /// <returns></returns>
        public static string GetPhysicalPathByWebRelPath(string webRelPath)
        {
            string appPath = Path.GetFullPath(HttpContext.Current.Server.MapPath("/"));
            return Path.GetFullPath(Path.Combine(appPath, webRelPath.Replace('\\', '/').TrimStart('/')));
        }

        /// <summary>
        /// 切换相对路径和绝对路径
        /// </summary>
        /// <param name="path">需转化的目录</param>
        /// <param name="useRel">是否转化为相对路径 默认true</param>
        /// <param name="rootPath">根目录默认为空，如果为空使用默认程序的根目录</param>
        /// <returns></returns>
        public static string SwitchAbsAndRelPath(string path, bool useRel = true, string rootPath = "")
        {
            if (FilterUtil.IsEmptyWithTrim(ref rootPath))
            {
                rootPath = DirectoryUtil.GetCurAppDirPath();
            }
            rootPath = rootPath.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);
            if (useRel)
            {
                if (path.StartsWith(rootPath))
                {
                    return StrUtil.SafeSub(path, rootPath.Length, path.Length - rootPath.Length).TrimStart(Path.DirectorySeparatorChar);
                }
                return path;
            }
            else
            {
                if (!path.StartsWith(rootPath))
                {
                    return Path.GetFullPath(Path.Combine(rootPath, path));
                }
                return Path.GetFullPath(path);
            }

        }
    }
}
