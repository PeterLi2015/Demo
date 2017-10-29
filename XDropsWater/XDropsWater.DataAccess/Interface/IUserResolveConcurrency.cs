using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// 数据冲突处理接口
    /// </summary>
    public interface IConcurrencyResolveHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentValues"></param>
        /// <param name="databaseValues"></param>
        /// <param name="resolvedValues"></param>
        void HandleUserResolveConcurrency(DbPropertyValues currentValues,
                                       DbPropertyValues databaseValues,
                                       DbPropertyValues resolvedValues);
    }
}
