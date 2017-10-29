using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XDropsWater.CrossCutting.String
{
    //字符串分割，及组合类
    public class StrValidationContainer
    {
        /// <summary>
        /// 用分隔符组合字符串
        /// </summary>
        public static string Combine(char escapeChar, char splitChar, string[] strArr)
        {
            StrValidationContainer sse = new StrValidationContainer(escapeChar, splitChar);
            return sse.Combine(strArr);
        }
        /// <summary>
        /// 分隔字符串D
        /// </summary>
        public static string[] Split(char escapeChar, char splitChar, string str)
        {
            StrValidationContainer sse = new StrValidationContainer(escapeChar, splitChar);
            return sse.Split(str);
        }

        public StrValidationContainer() { }
        public StrValidationContainer(char escapeChar, char splitChar)
        {
            CheckEscAndApc(escapeChar, splitChar);
            this.escapeChar = escapeChar;
            this.splitChar = splitChar;
        }

        /// <summary>
        /// 用分隔符组合字符串
        /// </summary>
        public string Combine(params string[] strArr)
        {
            string[] tempArr = new string[strArr.Length];
            string escapeStr = EscapeChar.ToString(), splitStr = SplitChar.ToString();
            string doubleEscStr = escapeStr + escapeStr;
            string escSplitStr = escapeStr + splitStr;
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = strArr[i].Replace(escapeStr, doubleEscStr).Replace(splitStr, escSplitStr);
            }
            return string.Join(splitStr, tempArr);
        }

        /// <summary>
        /// 分隔字符串
        /// </summary>
        public string[] Split(string str)
        {
            string escapeStr = EscapeChar.ToString(), splitStr = SplitChar.ToString();
            string doubleEscStr = escapeStr + escapeStr;
            string escSplitStr = escapeStr + splitStr;

            string pattern =
                string.Format(@"(?<=^|[^{0}]({0}{0})*){1}",
                Regex.Escape(escapeStr), Regex.Escape(splitStr));
            MatchCollection matchCol = Regex.Matches(str, pattern);
            if (matchCol.Count == 0) return new string[] { str.Replace(doubleEscStr, escapeStr).Replace(escSplitStr, splitStr) };
            List<string> list = new List<string>();

            int index = 0;
            for (int i = 0; i < matchCol.Count; i++)
            {
                list.Add(str.Substring(index, matchCol[i].Index - index));
                index = matchCol[i].Index + matchCol[i].Value.Length;
            }
            list.Add(str.Substring(index, str.Length - index));

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Replace(doubleEscStr, escapeStr).Replace(escSplitStr, splitStr);
            }
            return list.ToArray();
        }

        private void CheckEscAndApc(char escapeChar, char splitChar)
        {
            if (escapeChar == splitChar)
            {
                throw new Exception("转义符和分隔符不能相同，会使分割组合不准确.");
            }
        }

        private char escapeChar = '$';
        /// <summary>
        /// 转义符
        /// </summary>
        public char EscapeChar
        {
            get { return escapeChar; }
            set
            {
                CheckEscAndApc(value, SplitChar);
                escapeChar = value;
            }
        }


        private char splitChar = '|';
        /// <summary>
        /// 分割符
        /// </summary>
        public char SplitChar
        {
            get { return splitChar; }
            set
            {
                CheckEscAndApc(EscapeChar, value);
                splitChar = value;
            }
        }
    }
}
