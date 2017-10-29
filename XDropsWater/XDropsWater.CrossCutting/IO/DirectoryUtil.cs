using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.IO
{
    public class DirectoryUtil
    {
        /// <summary>
        /// 获得当前程序运行的所在文件夹
        /// </summary>
        public static string GetCurAppDirPath()
        {
            if (DotNetUtil.IsHttpRequest())
            {
                return Path.GetFullPath(System.Web.HttpContext.Current.Server.MapPath("~"));
            }
            else
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        /// <summary>
        /// 检查文件夹路径是否存在 如不存在创建它
        /// </summary>
        /// <param name="dirPath">需创建的文件夹路径</param>
        /// <returns>文件夹DirectoryInfo对象</returns>
        public static DirectoryInfo CreateOpenDir(string dirPath)
        {
            if (Directory.Exists(dirPath)) return new DirectoryInfo(dirPath);
            return Directory.CreateDirectory(dirPath);
        }

        /// <summary>
        /// 检查文件夹路径是否存在 如不存在创建它,如果发生异常使用默认路径
        /// </summary>
        /// <param name="dirPath">需创建的文件夹路径</param>
        /// <returns>文件夹DirectoryInfo对象</returns>
        public static DirectoryInfo CreateOpenDir(ref string dirPath, string defPath)
        {
            try
            {
                if (Directory.Exists(dirPath)) return new DirectoryInfo(dirPath);
                return Directory.CreateDirectory(dirPath);
            }
            catch (Exception)
            {
                dirPath = defPath;
                return CreateOpenDir(dirPath);
            }
        }

        /// <summary>
        /// 清楚指定目录下得空目录
        /// </summary>
        /// <param name="dirPath">需要清楚空</param>
        public static void DelEmptyDir(string dirPath)
        {
            string[] childDirArr = Directory.GetDirectories(dirPath);
            foreach (string dir in childDirArr)
            {
                DelEmptyDir(dir);
            }
            string[] chilFileArr = Directory.GetFiles(dirPath);
            //判断子文件夹和子文件是否为空
            if (childDirArr.Length == 0 && chilFileArr.Length == 0)
            {
                try
                {
                    Directory.Delete(dirPath, true);
                }
                catch { }
            }
        }

        /// <summary>
        /// 移动文件夹下所有文件
        /// </summary>
        public static void MoveDirFiles(string sourceDir, string targetDir, bool ignoreException)
        {
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                try
                {
                    string targetFilePath = Path.Combine(targetDir, Path.GetFileName(file));
                    File.Delete(targetFilePath);
                    File.Move(file, targetFilePath);
                }
                catch (Exception ex)
                {
                    if (ignoreException) continue;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 拷贝文件夹下所有文件
        /// </summary>
        public static void CopyDirFiles(string sourceDir, string targetDir, bool ignoreException)
        {
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                try
                {
                    string targetFilePath = Path.Combine(targetDir, Path.GetFileName(file));
                    File.Copy(file, targetFilePath, true);
                }
                catch (Exception ex)
                {
                    if (ignoreException) continue;
                    throw ex;
                }
            }
        }

    }
}
