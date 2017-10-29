using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XDropsWater.CrossCutting.String
{
    /// <summary>
    /// 验证扩展类
    /// </summary>
    public static class ValidateEx
    {
        /// <summary>
        /// 是否为数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[-+]?\d+(\.\d+)?$");
        }

        /// <summary>
        /// 检查字符串是否为空 如果字符串不为null，调用Trim后再检查是否为空字符串
        /// </summary>
        public static bool IsEmptyNullStrWithTrim(string str)
        {
            return string.IsNullOrEmpty(FilterUtil.EscapeNullTrim(str));
        }

        /// <summary>
        /// 过滤内容
        /// </summary>
        /// <param name="content">验证数据</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool LimitedDigital<T>(string content, T min, T max)
        {
            if (!IsNumeric(content)) return false;
            try
            {
                checked
                {
                    switch (typeof(T).Name)
                    {
                        case "Int32":
                            int numInt = int.Parse(content);
                            if (numInt < Convert.ToInt32(min) || numInt > Convert.ToInt32(max)) return false;
                            return true;
                        case "Single":
                            float numFloat = float.Parse(content);
                            if (numFloat < Convert.ToSingle(min) || numFloat > Convert.ToSingle(max)) return false;
                            return true;
                    }
                }
            }
            catch { return false; }
            throw new Exception("还未定义此数据类型的验证:" + typeof(T).Name);
        }

        #region 网络相关验证
        /// <summary>
        /// 验证IP格式
        /// </summary>
        /// <param name="content">验证内容</param>
        /// <returns>是否匹配</returns>
        public static bool ValidateIPAddrss(string content)
        {
            return Regex.IsMatch(content, @"^((25[0-5]|2[0-4]\d|1\d\d|[1-9]\d|\d)\.){3}(25[0-5]|2[0-4]\d|1\d\d|[1-9]\d|[1-9])$");
        }
        /// <summary>
        /// 验证网络端口
        /// </summary>
        /// <param name="content">验证内容</param>
        /// <returns>是否匹配</returns>
        public static bool ValidateNetPort(string content)
        {
            return Regex.IsMatch(content, @"^\d+$");
        }

        /// <summary>
        /// 验证是否为URL字符串
        /// </summary>
        /// <param name="url">需验证的字符串</param>
        /// <returns>是否合法</returns>
        public static bool ValidateUrl(string url)
        {
            Uri uriTemp;
            return Uri.TryCreate(url, UriKind.Absolute, out uriTemp);
        }
        #endregion

        /// <summary>
        /// 验证是否包含自定非法字符
        /// </summary>
        /// <param name="content">验证内容</param>
        /// <param name="illigalCharArr">非法字符</param>
        /// <returns>是否匹配</returns>
        public static bool ValidateIllegalChars(string content, char[] illigalCharArr)
        {
            if (illigalCharArr == null || illigalCharArr.Length == 0) return true;
            StringBuilder sb = new StringBuilder();
            foreach (char c in illigalCharArr)
            {
                sb.Append(c);
            }
            string pattern = string.Format("[{0}]", Regex.Escape(sb.ToString()));
            return Regex.IsMatch(content, pattern);
        }

        /// <summary>
        /// 验证字符串类型，多种类型可以用"|"复合使用StringType
        /// </summary>
        public static bool ValidateStringType(string strSource, StringType stringType)
        {
            string legalString = "_\\-\\*\\%\\$#@!\\^\\(\\)\\?\":;\\.\\{\\[\\]\\|\\\\";//无法使用Regex.Escape
            return ValidateStringType(strSource, stringType, legalString);
        }

        /// <summary>
        /// 验证字符串类型，多种类型可以用"|"复合使用StringType,StringType为LegalChar使用的字符,必要时可以自己修改legalString
        /// </summary>
        public static bool ValidateStringType(string strSource, StringType stringType, string legalString)
        {
            if (string.IsNullOrEmpty(strSource))
            {
                return false;
            }
            strSource = strSource.Trim();
            string pattern = @"^(";
            if ((stringType | StringType.Chinese) == stringType)
            {
                pattern += @"|[\u4E00-\u9FA5]";
            }
            if ((stringType | StringType.Numeric) == stringType)
            {
                pattern += @"|[0-9]";
            }
            if ((stringType | StringType.Letter) == stringType)
            {
                pattern += @"|[A-Za-z]";
            }
            if ((stringType | StringType.LegalSymbol) == stringType)
            {
                pattern += @"|[" + legalString + "]";
            }
            pattern += ")+$";
            return ValidateByPattern(strSource, pattern);
        }

        /// <summary>
        /// 用正则验证
        /// </summary>
        public static bool ValidateByPattern(string strSource, string pattern)
        {
            if (string.IsNullOrEmpty(strSource))
            {
                return false;
            }
            strSource = strSource.Trim();
            Match match = Regex.Match(strSource, pattern);
            if (string.IsNullOrEmpty(match.Value))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //待字符串类型枚举
        public enum StringType
        {
            /// <summary>
            /// 中文
            /// </summary>
            Chinese = 1,
            /// <summary>
            /// 字母
            /// </summary>
            Letter = 2,
            /// <summary>
            /// 数字
            /// </summary>
            Numeric = 4,
            /// <summary>
            /// 合法符号
            /// </summary>
            LegalSymbol = 8
        }
    }
}
