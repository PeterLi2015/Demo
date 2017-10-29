using System;

namespace XDropsWater.DataAccess.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreateUpdateLogEntity<TID> where TID : struct
    {
        /// <summary>
        /// 创建人相关ID
        /// </summary>
        TID? CreateBy { get; set; }
        /// <summary>
        /// 更新人相关ID
        /// </summary>
        TID? UpdateBy { get; set; }
        /// <summary>
        /// 创建相关时间
        /// </summary>
        DateTime? CreateOn { get; set; }
        /// <summary>
        /// 更新相关时间
        /// </summary>
        DateTime? UpdateOn { get; set; }
    }
}
