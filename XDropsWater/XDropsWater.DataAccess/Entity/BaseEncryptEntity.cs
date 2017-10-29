
using System.ComponentModel.DataAnnotations.Schema;
using XDropsWater.Dal.DataAccess.Entity;
using XDropsWater.CrossCutting.Encrypt;
using XDropsWater.CrossCutting.String;
namespace XDropsWater.DataAccess.Entity
{
    /// <summary>
    /// base class of all encrypted entitys
    /// </summary>
    public class BaseEncryptEntity<TID> : BaseEntity<TID>, IEncryptableEntity
        where TID : struct
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseEncryptEntity()
        {
            this.UseEncryption = true;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        public const string ENCRYPT_DATA_PREFIX = "X_SECRET_";
        /// <summary>
        /// 
        /// </summary>
        public const string ENCRYPT_KEY = "XCIICZJX";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsEncrypted(string data)
        {
            return data.StartsWith(ENCRYPT_DATA_PREFIX);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string SwitchEncryptDecrypt(string data)
        {
            if (UseEncryption)
            {
                if (IsEncrypted(data)) return data;
                return ENCRYPT_DATA_PREFIX + EncrypterUtil.AESEncrypt(data, ENCRYPT_KEY);
            }
            else
            {
                if (!IsEncrypted(data)) return data;
                return EncrypterUtil.AESDecrypt(
                    StrUtil.SafeSub(data, data.Length - ENCRYPT_DATA_PREFIX.Length, true), ENCRYPT_KEY);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public bool UseEncryption { get; set; }
    }
}
