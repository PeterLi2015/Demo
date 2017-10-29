using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XDropsWater.CrossCutting.String
{
    public static class FilterUtil
    {
        /// <summary>
        /// 过滤null字符和两边的空格，或末尾空白字符 
        /// </summary>
        /// <param name="content">需过滤的内容</param>
        /// <returns>过滤结果</returns>
        public static bool IsEmptyWithTrim(string content)
        {
            content = EscapeNullTrim(content);
            return string.IsNullOrEmpty(content.ToString());
        }

        /// <summary>
        /// 过滤null字符和两边的空格，或末尾空白字符 
        /// </summary>
        /// <param name="content">需过滤的内容</param>
        /// <returns>过滤结果</returns>
        public static bool IsEmptyWithTrim(ref string content)
        {
            content = EscapeNullTrim(content);
            return string.IsNullOrEmpty(content.ToString());
        }

        /// <summary>
        /// 过滤null字符和两边的空格，或末尾空白字符
        /// </summary>
        /// <param name="content">需过滤的内容obj</param>
        /// <returns>过滤结果</returns>
        public static string EscapeNullTrim(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString().Trim();
        }

        /// <summary>
        /// 过滤null
        /// </summary>
        /// <param name="content">需过滤的内容obj</param>
        /// <returns>过滤结果</returns>
        public static string EscapeNull(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString();
        }

        /// <summary>
        /// 过滤null字符和两边的空格，或末尾空白字符
        /// </summary>
        /// <param name="content">需过滤的内容obj对象</param>
        /// <returns>过滤结果</returns>
        public static string GetLegalStr(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString().Trim();
        }

        /// <summary>
        /// 获得合法格式路径
        /// </summary>
        /// <param name="path">需要过滤的路径</param>
        /// <returns>过滤后的路径</returns>
        public static string GetlegalPath(string path)
        {
            path = EscapeNullTrim(path);
            if (string.IsNullOrEmpty(path)) return string.Empty;
            foreach (char c in Path.GetInvalidPathChars())
            {
                path = path.Replace(c.ToString(), string.Empty);
            }
            return path;
        }

        /// <summary>
        /// 获得合法格式文件名
        /// </summary>
        /// <param name="path">需要过滤的文件名</param>
        /// <returns>过滤后的文件名</returns>
        public static string GetlegalFileName(string fileName)
        {
            fileName = EscapeNullTrim(fileName);
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), string.Empty);
            }
            return fileName;
        }
    }
}
