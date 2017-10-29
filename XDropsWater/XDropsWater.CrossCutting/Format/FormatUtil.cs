using XDropsWater.CrossCutting.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Format
{
    public static class FormatUtil
    {
        /// <summary>
        /// 把字符串转化成值类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T StrToValueType<T>(string value)
            where T : struct
        {
            if (typeof(T) == typeof(bool))
            {
                bool temp = ConvertStrToBool(value);
                return (T)Convert.ChangeType(temp, typeof(bool));
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 集合转化
        /// </summary>
        public static OutT[] ConvertTo<T, OutT>(T[] values, Func<T, OutT> func)
        {
            var fValues = new OutT[values.Length];
            for (int i = 0; i < fValues.Length; i++)
            {
                fValues[i] = func(values[i]);
            }
            return fValues;
        }
        /// <summary>
        /// 将字符格式对象转化成时间
        /// </summary>
        /// <param name="dtStrObj"></param>
        /// <returns></returns>
        public static DateTime ConvertStrToTime(object dtStrObj, string formatter = "")
        {
            string tempStr = FilterUtil.EscapeNullTrim(dtStrObj);
            if (FilterUtil.IsEmptyWithTrim(ref formatter))
            {
                return DateTime.Parse(tempStr);
            }
            else
            {
                return DateTime.ParseExact(tempStr, formatter,
                        System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 把字符串型时间object对象转化成时间对象，转化错误时，使用dtError作为返回值
        /// </summary>
        /// <param name="dtStrObj">字符串型时间object对象</param>
        /// <param name="dtError">转化错误时返回的时间对象</param>
        /// <param name="formatter">转化转化字符格式 比如 yyyyMMdd</param>
        /// <returns>转化后的时间对象</returns>
        public static DateTime ConvertStrToTime(object dtStrObj, DateTime dtError, string formatter = "")
        {
            string tempStr = FilterUtil.EscapeNullTrim(dtStrObj);
            if (FilterUtil.IsEmptyWithTrim(ref formatter))
            {
                DateTime.TryParse(tempStr, out dtError);
                return dtError;
            }
            else
            {
                DateTime.TryParseExact(tempStr, formatter,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dtError);
                return dtError;
            }
        }
        /// <summary>
        /// 获得标准时间字符串
        /// </summary>
        /// <param name="dt">时间对象</param>
        /// <returns>格式化后的时间字符串</returns>
        public static string GetTimeStandardStr(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 把字符串型对象转化成bool，转化失败返回 false
        /// </summary>
        /// <param name="boolStrObj">字符串型对象</param>
        /// <returns>转化后的bool值</returns>
        public static bool ConvertStrToBool(object boolStrObj)
        {
            string tempStr = FilterUtil.EscapeNullTrim(boolStrObj);
            bool result = false;
            bool.TryParse(tempStr, out result);
            return result;
        }

        /// <summary>
        /// 把图片转化成byte数组
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns>byte数组</returns>
        public static byte[] GetImageByteArr(string imagePath)
        {
            using (Image img = Image.FromFile(imagePath))
            {
                return GetImageByteArr(img);
            }
        }
        /// <summary>
        /// 把图片转化成byte数组
        /// </summary>
        /// <param name="imagePath">图片对象</param>
        /// <returns>byte数组</returns>
        public static byte[] GetImageByteArr(Image image)
        {
            if (image == null) return new byte[] { };
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 获得图片对象根据字节 效率不是很高 
        /// </summary>
        /// <param name="dataArr">字节数据</param>
        /// <returns>图片对象</returns>
        public static Image GetImageFromByteArr(byte[] dataArr)
        {
            if (dataArr == null || dataArr.Length == 0) return null;
            MemoryStream ms = new MemoryStream(dataArr);
            return Image.FromStream(ms);
        }

        private static List<string> idList = new List<string>();
        private static int idIndex = 0;
        /// <summary>
        /// 获取唯一的时间随机数
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeUniqueID()
        {
            lock (idList)
            {
                if (idIndex == 1000)
                {
                    Thread.Sleep(1);
                    idIndex = 0;
                }
                string id = DateTime.Now.ToString("yyMMddHHmmssfff") +
                    (idIndex++).ToString().PadLeft(3, '0');
                return id;
            }
        }

        #region 内存表和xml的相互转化
        /// <summary>
        /// 把表转化成xml
        /// </summary>
        public static string ConvertTableToXml(DataTable dt, XmlWriteMode xmlWriteMode, bool writeHierarchy)
        {
            using (StringWriter sw = new StringWriter())
            {
                dt.WriteXml(sw, xmlWriteMode, writeHierarchy);
                return sw.ToString();
            }
        }

        /// <summary>
        /// 把xml转化成内存表
        /// </summary>
        /// <param name="xmlData">xml数据字符串</param>
        /// <returns>内存表对象实例</returns>
        public static DataTable ConvertXmlToTable(string xmlData)
        {
            DataTable dt = new DataTable();
            ConvertXmlToTable(dt, xmlData);
            return dt;
        }

        /// <summary>
        /// 把xml转化成内存表
        /// </summary>
        /// <param name="dt">内存表对象实例</param>
        /// <param name="xmlData">xml数据字符串</param>
        public static void ConvertXmlToTable(DataTable dt, string xmlData)
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                dt.ReadXml(sr);
            }
        }

        /// <summary>
        /// 把数据集转化成xml
        /// </summary>
        /// <param name="ds">数据集对象</param>
        /// <param name="xmlWriteMode">指定如何从 System.Data.DataSet 写入 XML 数据和关系架构。</param>
        /// <returns>xml字符串</returns>
        public static string ConvertDataSetToXml(DataSet ds, XmlWriteMode xmlWriteMode)
        {
            using (StringWriter sw = new StringWriter())
            {
                ds.WriteXml(sw, xmlWriteMode);
                return sw.ToString();
            }
        }

        /// <summary>
        /// 把xml转化成数据集
        /// </summary>
        /// <param name="xmlData">xml字符串</param>
        /// <returns>数据集对象</returns>
        public static DataSet ConvertXmlToDataSet(string xmlData)
        {
            DataSet ds = new DataSet();
            ConvertXmlToDataSet(ds, xmlData);
            return ds;
        }

        /// <summary>
        /// 把xml转化成数据集
        /// </summary>
        /// <param name="ds">数据集对象</param>
        /// <param name="xmlData">xml字符串</param>
        public static void ConvertXmlToDataSet(DataSet ds, string xmlData)
        {
            using (StringReader sr = new StringReader(xmlData))
            {
                ds.ReadXml(sr, XmlReadMode.ReadSchema);
            }
        }
        #endregion
    }
}
