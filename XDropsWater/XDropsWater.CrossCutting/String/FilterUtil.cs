using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XDropsWater.CrossCutting.String
{
    public static class FilterUtil
    {
        /// <summary>
        /// ����null�ַ������ߵĿո񣬻�ĩβ�հ��ַ� 
        /// </summary>
        /// <param name="content">����˵�����</param>
        /// <returns>���˽��</returns>
        public static bool IsEmptyWithTrim(string content)
        {
            content = EscapeNullTrim(content);
            return string.IsNullOrEmpty(content.ToString());
        }

        /// <summary>
        /// ����null�ַ������ߵĿո񣬻�ĩβ�հ��ַ� 
        /// </summary>
        /// <param name="content">����˵�����</param>
        /// <returns>���˽��</returns>
        public static bool IsEmptyWithTrim(ref string content)
        {
            content = EscapeNullTrim(content);
            return string.IsNullOrEmpty(content.ToString());
        }

        /// <summary>
        /// ����null�ַ������ߵĿո񣬻�ĩβ�հ��ַ�
        /// </summary>
        /// <param name="content">����˵�����obj</param>
        /// <returns>���˽��</returns>
        public static string EscapeNullTrim(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString().Trim();
        }

        /// <summary>
        /// ����null
        /// </summary>
        /// <param name="content">����˵�����obj</param>
        /// <returns>���˽��</returns>
        public static string EscapeNull(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString();
        }

        /// <summary>
        /// ����null�ַ������ߵĿո񣬻�ĩβ�հ��ַ�
        /// </summary>
        /// <param name="content">����˵�����obj����</param>
        /// <returns>���˽��</returns>
        public static string GetLegalStr(object content)
        {
            if (content == null || DBNull.Value == content) return string.Empty;
            return content.ToString().Trim();
        }

        /// <summary>
        /// ��úϷ���ʽ·��
        /// </summary>
        /// <param name="path">��Ҫ���˵�·��</param>
        /// <returns>���˺��·��</returns>
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
        /// ��úϷ���ʽ�ļ���
        /// </summary>
        /// <param name="path">��Ҫ���˵��ļ���</param>
        /// <returns>���˺���ļ���</returns>
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
