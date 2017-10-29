using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XDropsWater.CrossCutting.Encrypt
{
    public class EncrypterUtil
    {
        /// <summary>
        /// 编码url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string EncodeUrl(string url, Encoding encoding)
        {
            return HttpUtility.UrlEncode(url, encoding);
        }
        /// <summary>
        /// 解码url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DecodeUrl(string uslCode, Encoding encoding)
        {
            return HttpUtility.UrlDecode(uslCode, encoding);
        }

        /// <summary> 
        /// 进行DES加密。 
        /// </summary> 
        /// <param name="pToEncrypt">要加密的字符串。</param> 
        /// <param name="encryKey">加密密钥关键字。</param> 
        /// <returns>以Base64格式返回的加密字符串。</returns> 
        public static string DESEncrypt(string pToEncrypt, string encryKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = ASCIIEncoding.ASCII.GetBytes(encryKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(encryKey);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary> 
        /// 进行DES解密。 
        /// </summary> 
        /// <param name="pToDecrypt">要解密的以Base64</param> 
        /// <param name="encryKey">加密密钥关键字。</param> 
        /// <returns>已解密的字符串。</returns> 
        public static string DESDecrypt(string pToDecrypt, string encryKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(encryKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(encryKey);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        /// <summary>
        /// 获得md5值
        /// </summary>
        /// <param name="pData_str">需要获得md5的文本</param>
        /// <param name="pEncryptType_int">MD5长度 16或32</param>
        /// <returns>MD5值</returns>
        public static string MD5Encrypt(string pData_str, int pEncryptType_int)
        {
            if (pEncryptType_int != 16 && pEncryptType_int != 32)
            {
                throw new ArgumentException("Length参数无效,只能为16位或32位");
            }
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] b = MD5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pData_str));
            System.Text.StringBuilder StrB = new System.Text.StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                StrB.Append(b[i].ToString("x").PadLeft(2, '0'));
            }
            if (pEncryptType_int == 16)
            {
                return StrB.ToString(8, 16);
            }
            else
            {
                return StrB.ToString();
            }
        }

        //获取文件MD5
        public static string GetFileMD5(string filePath, int pEncryptType_int)
        {
            return MD5Encrypt(File.ReadAllText(filePath), pEncryptType_int);
        }

        /// <summary>
        /// 获得Sha1的哈希值
        /// </summary>
        /// <param name="content">需要求哈希的文本字符串</param>
        /// <returns>哈希值</returns>
        public static string GetSha1(string content)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(content);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
            return str_sha1_out;
        }
        /// <summary>
        /// 解密Base64
        /// </summary>
        public static string Base64Decrypt(string pData_str)
        {
            byte[] bpath = Convert.FromBase64String(pData_str);
            return System.Text.ASCIIEncoding.UTF8.GetString(bpath);
        }
        /// <summary>
        /// 加密成Base64
        /// </summary>
        public static string Base64Encrypt(string data)
        {
            byte[] dataArr = System.Text.ASCIIEncoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(dataArr);
        }

        /// <summary>
        /// 获得字符串的Crc16值
        /// </summary>
        /// <param name="data">字符串的数据</param>
        /// <returns>Crc16值</returns>
        public static ushort GetStrCrc16(string data)
        {
            Crc16 c = new Crc16();
            return c.ComputeChecksum(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// 获得字符串的Crc32值
        /// </summary>
        /// <param name="data">字符串的数据</param>
        /// <returns>Crc32值</returns>
        public static uint GetStrCrc32(string data)
        {
            Crc32 c = new Crc32();
            return c.ComputeChecksum(Encoding.UTF8.GetBytes(data));
        }

        #region RSA加密相关功能
        /// <summary>
        /// 生成RSA的公、私密钥
        /// </summary>
        /// <param name="publicKeyPath">公钥文件路径</param>
        /// <param name="privateKeyPath">私钥文件路径</param>
        public static void GenerateRSAKey(string publicKeyPath, string privateKeyPath)
        {
            RSACryptoServiceProvider rsaKeyGenerator = new RSACryptoServiceProvider(1024);
            string publickey = rsaKeyGenerator.ToXmlString(false);
            string privatekey = rsaKeyGenerator.ToXmlString(true);
            using (var writer = new StreamWriter(publicKeyPath, false, Encoding.UTF8))
            {
                writer.Write(publickey);
            }
            using (var writer = new StreamWriter(privateKeyPath, false, Encoding.UTF8))
            {
                writer.Write(privatekey);
            }
        }

        /// <summary>
        /// 对指定数据进行RSA加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="publicKeyPath"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string plainText, string publicKeyPath)
        {
            RSACryptoServiceProvider rsaToEncrypt = new RSACryptoServiceProvider();
            using (var sr = new System.IO.StreamReader(publicKeyPath, Encoding.UTF8))
            {
                rsaToEncrypt.FromXmlString(sr.ReadToEnd());
            }
            byte[] encryptData = rsaToEncrypt.Encrypt(System.Text.Encoding.UTF8.GetBytes(plainText), false);
            rsaToEncrypt.Clear();
            return Convert.ToBase64String(encryptData);

        }

        /// <summary>
        /// 将RSA加密的数据解密
        /// </summary>
        /// <param name="base64Data"></param>
        /// <param name="privateKeyPath"></param>
        /// <returns></returns>
        public static string RSADecrypt(string base64Data, string privateKeyPath)
        {
            Console.WriteLine(privateKeyPath);
            RSACryptoServiceProvider rsaToEncrypt = new RSACryptoServiceProvider();
            using (StreamReader sr = new System.IO.StreamReader(privateKeyPath, Encoding.UTF8))
            {
                rsaToEncrypt.FromXmlString(sr.ReadToEnd());
            }
            byte[] encryptData = rsaToEncrypt.Decrypt(Convert.FromBase64String(base64Data), false);
            rsaToEncrypt.Clear();
            return System.Text.Encoding.UTF8.GetString(encryptData);
        }

        

        #endregion

        #region AES加解密
        /// <summary> 
        /// 进行AES加密。 
        /// </summary> 
        /// <param name="pToEncrypt">要加密的字符串。</param> 
        /// <param name="encryKey">加密密钥关键字。</param> 
        /// <returns>以Base64格式返回的加密字符串。</returns> 
        public static string AESEncrypt(string pToEncrypt, string encryKey)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                encryKey = MD5Encrypt(encryKey, 16);
                aes.Key = ASCIIEncoding.ASCII.GetBytes(encryKey);
                aes.IV = ASCIIEncoding.ASCII.GetBytes(encryKey);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary> 
        /// 进行AES解密。 
        /// </summary> 
        /// <param name="pToDecrypt">要解密的以Base64</param> 
        /// <param name="encryKey">加密密钥关键字。</param> 
        /// <returns>已解密的字符串。</returns> 
        public static string AESDecrypt(string pToDecrypt, string encryKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            encryKey = MD5Encrypt(encryKey, 16);
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = ASCIIEncoding.ASCII.GetBytes(encryKey);
                aes.IV = ASCIIEncoding.ASCII.GetBytes(encryKey);
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        #endregion
    }
}
