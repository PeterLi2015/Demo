using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace XDropsWater.CrossCutting.String
{
    /// <summary>
    /// ��֤��չ��
    /// </summary>
    public static class ValidateEx
    {
        /// <summary>
        /// �Ƿ�Ϊ��ֵ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[-+]?\d+(\.\d+)?$");
        }

        /// <summary>
        /// ����ַ����Ƿ�Ϊ�� ����ַ�����Ϊnull������Trim���ټ���Ƿ�Ϊ���ַ���
        /// </summary>
        public static bool IsEmptyNullStrWithTrim(string str)
        {
            return string.IsNullOrEmpty(FilterUtil.EscapeNullTrim(str));
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="content">��֤����</param>
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
            throw new Exception("��δ������������͵���֤:" + typeof(T).Name);
        }

        #region ���������֤
        /// <summary>
        /// ��֤IP��ʽ
        /// </summary>
        /// <param name="content">��֤����</param>
        /// <returns>�Ƿ�ƥ��</returns>
        public static bool ValidateIPAddrss(string content)
        {
            return Regex.IsMatch(content, @"^((25[0-5]|2[0-4]\d|1\d\d|[1-9]\d|\d)\.){3}(25[0-5]|2[0-4]\d|1\d\d|[1-9]\d|[1-9])$");
        }
        /// <summary>
        /// ��֤����˿�
        /// </summary>
        /// <param name="content">��֤����</param>
        /// <returns>�Ƿ�ƥ��</returns>
        public static bool ValidateNetPort(string content)
        {
            return Regex.IsMatch(content, @"^\d+$");
        }

        /// <summary>
        /// ��֤�Ƿ�ΪURL�ַ���
        /// </summary>
        /// <param name="url">����֤���ַ���</param>
        /// <returns>�Ƿ�Ϸ�</returns>
        public static bool ValidateUrl(string url)
        {
            Uri uriTemp;
            return Uri.TryCreate(url, UriKind.Absolute, out uriTemp);
        }
        #endregion

        /// <summary>
        /// ��֤�Ƿ�����Զ��Ƿ��ַ�
        /// </summary>
        /// <param name="content">��֤����</param>
        /// <param name="illigalCharArr">�Ƿ��ַ�</param>
        /// <returns>�Ƿ�ƥ��</returns>
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
        /// ��֤�ַ������ͣ��������Ϳ�����"|"����ʹ��StringType
        /// </summary>
        public static bool ValidateStringType(string strSource, StringType stringType)
        {
            string legalString = "_\\-\\*\\%\\$#@!\\^\\(\\)\\?\":;\\.\\{\\[\\]\\|\\\\";//�޷�ʹ��Regex.Escape
            return ValidateStringType(strSource, stringType, legalString);
        }

        /// <summary>
        /// ��֤�ַ������ͣ��������Ϳ�����"|"����ʹ��StringType,StringTypeΪLegalCharʹ�õ��ַ�,��Ҫʱ�����Լ��޸�legalString
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
        /// ��������֤
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

        //���ַ�������ö��
        public enum StringType
        {
            /// <summary>
            /// ����
            /// </summary>
            Chinese = 1,
            /// <summary>
            /// ��ĸ
            /// </summary>
            Letter = 2,
            /// <summary>
            /// ����
            /// </summary>
            Numeric = 4,
            /// <summary>
            /// �Ϸ�����
            /// </summary>
            LegalSymbol = 8
        }
    }
}
