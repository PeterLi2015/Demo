using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// Execute String Sql Statement
    /// </summary>
    public interface ISql
    {
        /// <summary>
        /// Execute Sql Query By Specific SqlQuery
        /// </summary>
        /// <typeparam name="TEntity">Specific Generic Type</typeparam>
        /// <param name="a_SqlQuery">Sql Query String</param>
        /// <param name="a_Parameters">SQL Parameter(if Needed)</param>
        /// <returns>Enumerable Specific Generic Array</returns>
        IEnumerable<TEntity> ExecuteQuery<TEntity>(string a_SqlQuery, params object[] a_Parameters);

        /// <summary>
        /// Exucte Specific Sql Command
        /// </summary>
        /// <param name="a_SqlCommand">Sql Command String</param>
        /// <param name="a_Parameters">SQL Parameter(if Needed)</param>
        /// <returns>Effective Records Number</returns>
        int ExecuteCommand(string a_SqlCommand, params object[] a_Parameters);
    }
}
