using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using XDropsWater.DataAccess.Interface;

namespace XDropsWater.DataAccess.Encrypt
{
    /// <summary>
    /// Serial Encrypter
    /// </summary>
    public class SerialEncrypter : IEncrypter
    {
        /// <summary>
        /// Serialize Data to String
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="val">Object Data</param>
        /// <returns>Encrypt String</returns>
        public static string Serialize<T>(T val)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var textWriter = new StringWriter())
            {
                ser.Serialize(textWriter, val);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// DeSerialize String to Data
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="str">Encrypt String</param>
        /// <returns>Type Object</returns>
        public static T DeSerialize<T>(string str)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var textReader = new StringReader(str))
            {
                return (T)ser.Deserialize(textReader);
            }
        }

        /// <summary>
        /// Encrypt Generic Type
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="a_RealData">Data Object</param>
        /// <returns>Encrypted String</returns>
        public string Encrypt<T>(T a_RealData)
        {
            var str = Serialize<T>(a_RealData);
            str = new string(str.Reverse().ToArray());
            return str;
        }

        /// <summary>
        /// Decrypt String to Obj
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="a_EncryptedStr">Encrypted String</param>
        /// <returns>Data Object</returns>
        public T Decrypt<T>(string a_EncryptedStr)
        {
            var realStr = new string(a_EncryptedStr.Reverse().ToArray()); ;
            return (T)DeSerialize<T>(realStr);
        }
    }


}
