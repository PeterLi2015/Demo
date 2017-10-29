
namespace XDropsWater.DataAccess.Entity
{
    /// <summary>
    /// Encryptable entity interface
    /// </summary>
    public interface IEncryptableEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool IsEncrypted(string data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string SwitchEncryptDecrypt(string data);
        /// <summary>
        /// 是否使用加密
        /// </summary>
        bool UseEncryption { get; set; }
    }
}
