using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// Encrypt Data
    /// </summary>
    public interface IEncryptable
    {
        /// <summary>
        /// Encrypt Data
        /// </summary>
        void Encrypt<T>();

        /// <summary>
        /// Decrypt Data
        /// </summary>
        void Decrypt<T>();
    }
}
