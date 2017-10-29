using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace XDropsWater.CrossCutting
{
    /// <summary>
    /// 内存表扩展
    /// </summary>
    public class DataTableUtil
    {
        /// <summary>
        /// 获取列名集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string[] GetColumnNames(DataTable dt)
        {
            return dt.Columns.OfType<DataColumn>().Select(dc => dc.ColumnName).ToArray();
        }
    }
}
