using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Serialization
{
    public class SerializerUtil
    {
        /// <summary>
        /// Serialize Data to String
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="val">Object Data</param>
        /// <returns>Encrypt String</returns>
        public static string Serialize<T>(T val)
        {
            return Serialize(typeof(T), val);
        }

        public static string Serialize(Type type, object val)
        {
            using (var textWriter = new StringWriter())
            {
                var ser = new System.Xml.Serialization.XmlSerializer(type);
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
            return (T)DeSerialize(typeof(T), str);
        }

        public static object DeSerialize(Type type, string str)
        {
            using (var textReader = new StringReader(str))
            {
                var ser = new System.Xml.Serialization.XmlSerializer(type);
                return ser.Deserialize(textReader);
            }
        }
    }
}
