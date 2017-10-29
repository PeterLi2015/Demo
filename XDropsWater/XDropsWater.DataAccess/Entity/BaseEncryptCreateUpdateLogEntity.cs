using System;

namespace XDropsWater.DataAccess.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEncryptCreateUpdateLogEntity<TID> : BaseEncryptEntity<TID>, ICreateUpdateLogEntity<TID>
        where TID : struct
    {
        /// <summary>
        /// 创建人相关ID
        /// </summary>
        public TID? CreateBy { get; set; }
        /// <summary>
        /// 创建相关时间
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 更新人相关ID
        /// </summary>
        public TID? UpdateBy { get; set; }
        /// <summary>
        /// 更新相关时间
        /// </summary>
        public DateTime? UpdateOn { get; set; }
    }
}
