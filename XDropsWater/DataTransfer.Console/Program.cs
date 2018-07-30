using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://www.xsdhbkj.com");
            long timeSpan = GetTimestamp();
            var appKey = "PG6e31aa765fcb436b";
            var secret = "D9U7YY5D7FF2748AED89E90HJ88881E6";
            var sign = MD5Encrypt(appKey + secret + timeSpan);
            //var url = string.Format("api/userinfoapi?timestamp={0}&appkey={1}&sign={2}", timeSpan, appKey, sign);
            //HttpResponseMessage response = client.GetAsync(url).Result;
            //System.Console.WriteLine(response);
            //System.Console.Read();
            //var url = "api/userinfoapi";
            //// 创建JSON格式化器。
            //MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            //// Use the JSON formatter to create the content of the request body.
            //// 使用JSON格式化器创建请求体内容。
            //HttpContent content = new ObjectContent<Product>(product, jsonFormatter);

            //client.PostAsync(url, )
            //https://www.mgenware.com/blog/?p=334
        }
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            return System.Text.Encoding.Default.GetString(result);
        }
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);//ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
            return (long)ts.TotalMilliseconds; //精确到毫秒
            //return (long)ts.TotalSeconds;//获取10位
        }
    }
}
