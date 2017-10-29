using XDropsWater.CrossCutting.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.IO
{
    public class CsvUtil
    {
        /// <summary>
        /// 把内存表写入csv
        /// </summary>
        public static void WriteTable(string filePath, DataTable dt)
        {
            new CsvUtil(filePath).WriteTable(dt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public CsvUtil(string filePath)
            : this(filePath, Encoding.UTF8)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public CsvUtil(string filePath, Encoding encoding)
        {
            if (FilterUtil.IsEmptyWithTrim(ref filePath))
            {
                throw new ArgumentNullException(filePath, "文件路径不能为空.");
            }
            if (Path.GetExtension(filePath).ToLower() != ".csv")
            {
                throw new ArgumentNullException(filePath, "文件类型必须是csv.");
            }
            this.FilePath = filePath;
            this.Encoding = encoding;
        }

        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; protected set; }


        /// <summary>
        /// 获得格式化csv数据行
        /// </summary>
        public static string GetFormatCsvLine(params object[] cells)
        {
            string tempCellStr = string.Empty;
            string[] cellStrs = ArrayUtil.ChangeValue(cells, (value) =>
            {
                tempCellStr = FilterUtil.EscapeNullTrim(value);
                if (tempCellStr.IndexOf(',') > -1)
                {
                    tempCellStr = string.Format("\"{0}\"", tempCellStr.Replace("\"", "\"\""));
                }
                return tempCellStr;
            }).ToArray();
            return string.Format("{0}", string.Join(",", cellStrs));
        }

        /// <summary>
        /// 把内存表写入csv
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetName"></param>
        public virtual void WriteTable(DataTable dt)
        {
            var columns = DataTableUtil.GetColumnNames(dt);
            var header = GetFormatCsvLine(columns);
            using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var sw = new StreamWriter(fs, Encoding))
                {
                    sw.WriteLine(header);
                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        var row = dt.Rows[i];
                        var cells = new object[dt.Columns.Count];
                        for (var j = 0; j < dt.Columns.Count; j++)
                        {
                            cells[j] = row[j];
                        }
                        var value = GetFormatCsvLine(cells);
                        sw.WriteLine(value);
                    }
                }
            }
        }

        /// <summary>
        /// 读取所有csv行数据
        /// </summary>
        /// <returns></returns>
        public virtual string[][] ReadAllLines()
        {
            var lineDataList = new List<string[]>();
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(fs, Encoding))
                {
                    while (sr.Peek() >= 0)
                    {
                        var lineStr = sr.ReadLine();
                        try
                        {
                            lineDataList.Add(AnalyzeCsvLineData(lineStr));
                        }
                        catch (Exception)
                        {
                            throw new FormatException(string.Format("csv行数据[{0}]格式异常.", lineStr));
                        }

                    }
                }
            }
            return lineDataList.ToArray();
        }

        /// <summary>
        /// 解析csv行数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string[] AnalyzeCsvLineData(string lineStr)
        {
            var lineItemList = new List<string>();
            bool bInTheWord = false;
            bool bTransfer = false;//转义标识
            bool bMayBeQuoteBegin = (lineStr[0] == '"') ? true : false;//新字段引号开头标识

            string strCurWord = string.Empty;
            for (var nIndex = 0; nIndex < lineStr.Length; nIndex++)
            {
                #region 字符解析
                char curChar = lineStr[nIndex];
                switch (curChar)
                {
                    case '"':
                        if (bMayBeQuoteBegin)//如果是下一个字段的开始则其后的字符算在当前字段中
                        {
                            bMayBeQuoteBegin = false;
                            bInTheWord = true;
                        }
                        else
                        {
                            if (bInTheWord)//碰到第双数个引号(可能是第二个，可能是第四个。。。)
                            {
                                if (bTransfer)//判断是否转义
                                {
                                    bTransfer = false;
                                    strCurWord += curChar;
                                    continue;
                                }
                                if (nIndex < lineStr.Length - 1)//判断是转义还是当前字段的结束标识
                                {
                                    if (lineStr[nIndex + 1] == '"')
                                        bTransfer = true;
                                    else if (lineStr[nIndex + 1] == ',')
                                        bInTheWord = false;
                                    else
                                    {
                                        //如果都不是的话说明文件格式错误
                                        throw new FormatException(
                                            string.Format("\"引号语义不明确,位置:{0}.", nIndex));
                                    }
                                }
                                else
                                {
                                    if (bTransfer)
                                    {
                                        throw new FormatException(
                                            string.Format("\"引号转义不明确,位置:{0}.", nIndex));
                                    }
                                    if (bInTheWord)
                                    {
                                        bInTheWord = false;
                                    }
                                }
                            }
                            else
                            {
                                throw new FormatException(
                                    string.Format("存在引号的数据字段需要用一对双引号在字段首尾，不然视为格式错误,位置:{0}.", nIndex));
                            }
                        }
                        break;
                    case ',':
                        if (bInTheWord)//如果属于源数据，既非分隔符
                            strCurWord += curChar;
                        else
                        {
                            lineItemList.Add(strCurWord);
                            strCurWord = string.Empty;

                            bInTheWord = false;//当前字段完毕
                            bMayBeQuoteBegin = true;//上一个字段完
                        }
                        break;
                    default:
                        if (bMayBeQuoteBegin)//如果是下一个字段的开始则其后的字符算在当前字段中
                            bMayBeQuoteBegin = false;
                        strCurWord += curChar;
                        break;
                }
                #endregion
            }
            if (bInTheWord)
            {
                throw new FormatException("引号或数据项未结束.");
            }
            lineItemList.Add(strCurWord);//the last word
            return lineItemList.ToArray();
        }
    }
}
