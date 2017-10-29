using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Interface;

namespace XDropsWater.DataAccess.Encrypt
{
    /// <summary>
    /// Encryptable Entity Base Class
    /// </summary>
    public abstract class EncryptableEntity : IEncryptable
    {

        /// <summary>
        /// Inner Object Type
        /// </summary>
        [Serializable]
        public abstract class EncryptData
        {
        }

        /// <summary>
        /// Encrypt Data
        /// </summary>
        protected object m_EncryptData = null;

        /// <summary>
        /// Encrypter
        /// </summary>
        protected IEncrypter m_Encrypter = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a_Encrypter">Encrypter</param>
        public EncryptableEntity(IEncrypter a_Encrypter)
        {
            m_Encrypter = a_Encrypter;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected EncryptableEntity()
        {

        }

        /// <summary>
        /// Encrypt Object
        /// </summary>
        protected abstract EncryptData EncryptObj
        {
            get;
        }

        /// <summary>
        /// Encrypter
        /// </summary>
        [NotMapped]
        public IEncrypter Encrypter
        {
            get { return m_Encrypter; }
            set { m_Encrypter = value; }
        }

        /// <summary>
        /// Encrypt Data String
        /// </summary>
        public virtual string EncryptDataString
        {
            get;
            set;
        }

        /// <summary>
        /// Encrypt Data
        /// </summary>
        public virtual void Encrypt<T>()
        {
            CopyObjValueToEncryptData();
            EncryptDataString = m_Encrypter.Encrypt<T>((T)m_EncryptData);
        }

        /// <summary>
        /// Decrypt Data
        /// </summary>
        public virtual void Decrypt<T>()
        {
            m_EncryptData = (T)m_Encrypter.Decrypt<T>(EncryptDataString);
            CopyEncryptDataValueToObj();
        }

        /// <summary>
        /// Copy Obj Value To Encrypt Data
        /// </summary>
        protected abstract void CopyObjValueToEncryptData();

        /// <summary>
        /// Copy  Encrypt Data  To Obj Value
        /// </summary>
        protected abstract void CopyEncryptDataValueToObj();
    }
}
