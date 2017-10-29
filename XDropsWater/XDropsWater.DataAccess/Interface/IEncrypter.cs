using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// Interface of Encrypter
    /// </summary>
    public interface IEncrypter
    {
        /// <summary>
        /// Encrypt Generic Type
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="a_RealData">Data Object</param>
        /// <returns>Encrypted String</returns>
        string Encrypt<T>(T a_RealData);

        /// <summary>
        /// Decrypt String to Obj
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="a_EncryptedStr">Encrypted String</param>
        /// <returns>Data Object</returns>
        T Decrypt<T>(string a_EncryptedStr);
    }
}
