using XDropsWater.CrossCutting.Format;
using XDropsWater.CrossCutting.String;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.IO
{
    public static class FileUtil
    {
        private static string IMG_EXTENSIONS = ".jpg.png.bmp.jpeg.gif.tiff.raw.pcx.dxf.wmf.emf.lic.eps";
        /// <summary>
        /// 判断是否为图片文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否为图片文件路径</returns>
        public static bool IsImgFile(string path)
        {
            return IMG_EXTENSIONS.IndexOf(Path.GetExtension(path)) != -1;
        }


        /// <summary> 
        /// 取得一个文本文件流的编码方式。 
        /// </summary> 
        /// <param name="stream">文本文件流。</param> 
        /// <param name="defaultEncoding">默认编码方式。 
        /// 当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。 
        /// </param> 
        /// <returns></returns> 
        public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (stream != null && stream.Length >= 2)
            {
                //保存文件流的前4个字节 
                byte byte1 = 0;
                byte byte2 = 0;
                byte byte3 = 0;
                byte byte4 = 0;
                //保存当前Seek位置 
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);

                int nByte = stream.ReadByte();
                byte1 = Convert.ToByte(nByte);
                byte2 = Convert.ToByte(stream.ReadByte());
                if (stream.Length >= 3)
                {
                    byte3 = Convert.ToByte(stream.ReadByte());
                }
                if (stream.Length >= 4)
                {
                    byte4 = Convert.ToByte(stream.ReadByte());
                }
                //根据文件流的前4个字节判断Encoding 
                //Unicode {0xFF, 0xFE}; 
                //BE-Unicode {0xFE, 0xFF}; 
                //UTF8 = {0xEF, 0xBB, 0xBF}; 
                if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe 
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode 
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8 
                {
                    targetEncoding = Encoding.UTF8;
                }
                if (byte1 == 0xFE && byte2 == 0xFF && byte3 == 0x4E && byte4 == 0x2D)
                {
                    targetEncoding = Encoding.GetEncoding("UTF-16BE");
                }
                if (byte1 == 0xFF && byte2 == 0xFE && byte3 == 0x2D && byte4 == 0x4E)
                {
                    targetEncoding = Encoding.GetEncoding("UTF-16LE");
                }
                //恢复Seek位置       
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return targetEncoding;
        }

        /// <summary> 
        /// 取得一个文本文件流的编码方式。 
        /// </summary> 
        /// <param name="stream">文本路径。</param> 
        /// <param name="defaultEncoding">默认编码方式。 
        /// 当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。 
        /// </param> 
        /// <returns></returns> 
        public static Encoding GetEncoding(string filePath, Encoding defaultEncoding)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return GetEncoding(fs, defaultEncoding);
            }
        }


        /// <summary>
        /// 获得不存在的文件名 文件名以序号累加的方式
        /// </summary>
        /// <param name="filePath">需要检查路径名</param>
        /// <returns>返回不存在的文件名，如果存在以序号累加的方式修改文件名</returns>
        public static string GetNotExistFileNameWithSeqNum(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            string renameFilePath = filePath;
            int i = 1;
            while (File.Exists(renameFilePath))
            {
                renameFilePath =
                    Path.Combine(dir,
                    string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(filePath), i, Path.GetExtension(filePath)));
                i++;
            }
            return renameFilePath;
        }

        /// <summary>
        /// 给文件路径加上时间唯一ID
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileNameWithTimeID(string filePath)
        {
            return Path.Combine(Path.GetDirectoryName(filePath),
                    string.Format("{0}_{1}{2}",
                    Path.GetFileNameWithoutExtension(filePath), FormatUtil.GenerateTimeUniqueID(), Path.GetExtension(filePath)));
        }

        /// <summary>
        /// 把流写到指定文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        public static void WriteStreamToFile(Stream stream, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                while (true)
                {
                    var caches = new byte[64 * 1024];
                    var realSize = stream.Read(caches, 0, caches.Length);
                    fs.Write(caches, 0, realSize);
                    if (realSize < caches.Length)
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 获得程序集的路径
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyPath(Assembly assembly)
        {
            var preTag = "file:///";
            if (FilterUtil.IsEmptyWithTrim(assembly.CodeBase)) return string.Empty;
            return assembly.CodeBase.Substring(preTag.Length);
        }
        /// <summary>
        /// 获得程序集的路径
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public static string GetAssemblyPath(AssemblyName dllName)
        {
            var preTag = "file:///";
            if (dllName.CodeBase == null)
            {
                return GetAssemblyPath(Assembly.Load(dllName));
            }
            return dllName.CodeBase.Substring(preTag.Length);
        }
        /// <summary>
        /// 判断dll是否属于微软
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public static bool IsMicroSoftDll(string dllPath)
        {
            return FileVersionInfo.GetVersionInfo(dllPath).CompanyName.IndexOf(
                "Microsoft Corporation", StringComparison.OrdinalIgnoreCase) != -1;
        }

        /// <summary>
        /// 判断文件是否有读取权限
        /// </summary>
        public static bool HasFileReadRight(string fileFullName, FileShare sf = FileShare.ReadWrite)
        {
            if (!File.Exists(fileFullName))
            {
                throw new FileNotFoundException(string.Format("文件[{0}]不存在.", fileFullName));
            }
            try
            {
                using (var fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, sf))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
