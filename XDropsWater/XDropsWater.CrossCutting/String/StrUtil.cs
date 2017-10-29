using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Web;

namespace XDropsWater.CrossCutting.String
{
    /// <summary>
    /// string类的扩展
    /// </summary>
    public class StrUtil
    {
        /// <summary>
        /// 获得指定宽度省略字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="length">最长容纳长度</param>
        /// <returns>省略后的字符串</returns>
        public static string GetEllipsisString(string str, int length)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            string newStr = string.Empty;
            double tempLen = 3;
            foreach (char c in str)
            {
                if (tempLen > length)
                {
                    newStr += "...";
                    break;
                }
                if ((int)c <= 256)
                {
                    tempLen += 0.5;
                }
                else
                {
                    tempLen += 1;
                }
                newStr += c;
            }
            return newStr;
        }

        /// <summary>
        /// 获得字符串后几位字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetLastNumChars(string str, int num)
        {
            str = FilterUtil.EscapeNullTrim(str);
            if (str.Length <= num) return str;
            return str.Substring(str.Length - num, num);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="value">需截取的字符串</param>
        /// <param name="num">截取字符数</param>
        /// <param name="reverse">是否反向 默认false</param>
        /// <returns></returns>
        public static string SafeSub(string value, int num, bool reverse = false)
        {
            value = FilterUtil.EscapeNullTrim(value);
            if (value.Length <= num) return value;
            if (!reverse)
            {
                return value.Substring(0, num);
            }
            else
            {
                return value.Substring(value.Length - num, num);
            }
        }

        /// <summary>
        /// 安全截取字符串
        /// </summary>
        /// <param name="value">需截取的字符串</param>
        /// <param name="start">开始截取位置</param>
        /// <param name="num">截取字符数</param>
        /// <param name="reverse">是否反向 默认false</param>
        /// <returns></returns>
        public static string SafeSub(string value, int start, int num)
        {
            value = FilterUtil.EscapeNullTrim(value);
            if (start >= value.Length) return string.Empty;
            if (start + num > value.Length) return value.Substring(start, value.Length - start);
            return value.Substring(start, num);
        }

        #region 全半角切换
        /*
         在计算机屏幕上，一个汉字要占两个英文字符的位置，人们把一个英文字符所占的位置称为"半角"，
         * 相对地把一个汉字所占的位置称为"全角"。在汉字输入时，系统提供"半角"和"全角"两种不同的输入状态，
         * 但是对于英文字母、符号和数字这些通用字符就不同于汉字，在半角状态它们被作为英文字符处理；而在全角状态，
         * 它们又可作为中文字符处理。半角和全角切换方法：单击输入法工具条上的 按钮或按键盘上的Shift+Space键来切换。 
        （1）全角--指一个字符占用两个标准字符位置。
        汉字字符和规定了全角的英文字符及国标GB2312-80中的图形符号和特殊字符都是全角字符。一般的系统命令是不用全角字符的，
         * 只是在作文字处理时才会使用全角字符。
        （2）半角--指一字符占用一个标准的字符位置。
        通常的英文字母、数字键、符号键都是半角的，半角的显示内码都是一个字节。在系统内部，以上三种字符是作为基本代码处理的，
         * 所以用户输入命令和参数时一般都使用半角。
        B 全角与半角各在什么情况下使用？
        全角占两个字节，半角占一个字节。
        半角全角主要是针对标点符号来说的，全角标点占两个字节，半角占一个字节，而不管是半角还是全角，汉字都还是要占两个字节。
        在编程序的源代码中只能使用半角标点（不包括字符串内部的数据）
        在不支持汉字等语言的计算机上只能使用半角标点（其实这种情况根本就不存在半角全角的概念）
        对于大多数字体来说，全角看起来比半角大，当然这不是本质区别了。
        C 全角和半角的区别
        全角就是字母和数字等与汉字占等宽位置的字。半角就是ASCII方式的字符，在没有汉字输入法起做用的时候输入的字母数字和字符都是半角的。
        在汉字输入法出现的时候，输入的字母数字默认为半角，但是标点则是默认为全角，可以通过鼠标点击输入法工具条上的相应按钮来改变。
        D 关于“全角”和“半角”：
        全角：是指中GB2312-80（《信息交换用汉字编码字符集·基本集》）中的各种符号。
        半角：是指英文件ASCII码中的各种符号。
        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static string ReplaceToSB(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                switch ((int)c[i])
                {
                    case 12288:
                        c[i] = (char)32;
                        continue;
                    //case 8217:
                    //    c[i] = (char)39;
                    //    continue;
                    //case 8216:
                    //    c[i] = (char)39;
                    //    continue;
                    //case 8220:
                    //    c[i] = (char)34;
                    //    continue;
                    //case 8221:
                    //    c[i] = (char)34;
                    //    continue;
                    //case 8211:
                    //    c[i] = '-';
                    //    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        #endregion


        #region 小写金额转化为大写金额
        /// <summary>  
        /// 金额转大写
        /// </summary>  
        /// <param name="LowerMoney"></param>  
        /// <returns></returns>  
        public static string MoneyToChinese(string LowerMoney, bool AreYuan = false)
        {
            LowerMoney = (Math.Round(double.Parse(LowerMoney), 2)).ToString();
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数  
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数  
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4  
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    //case ".":
                    //    strUpart = "圆";
                    //    break;
                    //case "0":
                    //    strUpart = "零";
                    //    break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    //case 1:
                    //    strUpart = strUpart + "分";
                    //    break;
                    //case 2:
                    //    strUpart = strUpart + "角";
                    //    break;
                    //case 3:
                    //    strUpart = strUpart + "";
                    //    break;
                    //case 4:
                    //    strUpart = strUpart + "";
                    //    break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            //strUpper = strUpper.Replace("零拾", "零");
            //strUpper = strUpper.Replace("零佰", "零");
            //strUpper = strUpper.Replace("零仟", "零");
            //strUpper = strUpper.Replace("零零零", "零");
            //strUpper = strUpper.Replace("零零", "零");
            //strUpper = strUpper.Replace("零角零分", "元整");
            // strUpper = strUpper.Replace("零分", "整");
            //strUpper = strUpper.Replace("零角", "零");
            //strUpper = strUpper.Replace("零亿零万零圆", "亿圆");
            //strUpper = strUpper.Replace("亿零万零圆", "亿圆");
            //strUpper = strUpper.Replace("零亿零万", "亿");
            //strUpper = strUpper.Replace("零万零圆", "万圆");
            //strUpper = strUpper.Replace("零亿", "亿");
            //strUpper = strUpper.Replace("零万", "万");
            //strUpper = strUpper.Replace("零圆", "圆");
            //strUpper = strUpper.Replace("零零", "零");

            // 对壹圆以下的金额的处理  
            if (strUpper.Substring(0, 1) == "元")
            {
                //strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                // strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                // strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                // strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整" && AreYuan)
            {
                //  strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }




        #endregion

        #region 小写金额转化为大写金额
        /// <summary>  
        /// 金额转大写  目前默认的单位 是万元
        /// </summary>  
        /// <param name="temp"></param>  
        /// <returns></returns>  
        public static string MoneyToChineseToo(string temp)
        {
            if (string.IsNullOrEmpty(temp))
                return temp;
            temp = (decimal.Round(decimal.Parse(temp) * 10000, 0)).ToString();
            string[] units = new string[] { "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };
            string[] numeric = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            var res = "";
            while (!string.IsNullOrEmpty(temp))
            {
                // 遍历一行中所有数字   
                for (int k = -1; temp.Length > 0; k++)
                {
                    // 解析最后一位   
                    int j = int.Parse(temp.Substring(temp.Length - 1, 1));
                    var rtemp = numeric[j];

                    // 数值不是0且不是个位 或者是万位或者是亿位 则去取单位   
                    if (j != 0 && k != -1 || k % 8 == 3 || k % 8 == 7)
                    {
                        rtemp += units[k % 8];
                    }

                    // 拼在之前的前面   
                    res = rtemp + res;

                    // 去除最后一位   
                    temp = temp.Substring(0, temp.Length - 1);
                }
                // 去除后面连续的零零..   
                while (res.EndsWith(numeric[0]))
                {
                    res = res.Substring(0, res.LastIndexOf(numeric[0]));
                }
                // 将零零替换成零   
                while (res.IndexOf(numeric[0] + numeric[0]) != -1)
                {
                    res = res.Replace(numeric[0] + numeric[0], numeric[0]);
                }
                // 将 零+某个单位 这样的窜替换成 该单位 去掉单位前面的零   
                for (int m = 1; m < units.Length; m++)
                {
                    res = res.Replace(numeric[0] + units[m], units[m]);
                }
                temp = string.Empty;
                if (!string.IsNullOrEmpty(res))
                {
                    res = res + "元整";
                }
            }
            return res;

        }
        #endregion

        /// <summary>
        /// 去掉html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",
            RegexOptions.IgnoreCase);
            //删除HTML 
            //Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",
            //RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"–>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!–.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",
            RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<font>", "");
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",
            RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
    }
}
