using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.CrossCutting
{
    /// <summary>
    /// 时间操作扩展功能
    /// </summary>
    public class DateTimeUtil
    {
        /// <summary>
        /// SqlServer 最小时间
        /// </summary>
        public static DateTime SqlServerMinDT = DateTime.Parse("1900-01-01");

        /// <summary>
        /// 根据 当天日期的前后天数，获取时间集合
        /// </summary>
        public static DateTime[] GetDaysByPreLastDays(int preDays, int lastDays)
        {
            DateTime dt = DateTime.Now;
            return GetDaysByPreLastDays(dt, preDays, lastDays);
        }

        /// <summary>
        /// 根据 指定日期的前后天数，获取时间集合
        /// </summary>
        public static DateTime[] GetDaysByPreLastDays(DateTime dt, int preDays, int lastDays)
        {
            return GetDaysByFromTo(dt.AddDays((-1) * preDays), dt.AddDays(lastDays));
        }

        /// <summary>
        /// 根据开始，结束时间获取时间集合
        /// </summary>
        public static DateTime[] GetDaysByFromTo(DateTime fromDt, DateTime toDt)
        {
            if (fromDt > toDt)
            {
                throw new Exception("获取日期区间时，起始时间不能大于结束时间");
            }
            var dtList = new List<DateTime>();
            while (fromDt <= toDt)
            {
                dtList.Add(fromDt);
                fromDt = fromDt.AddDays(1);
            }
            return dtList.ToArray();
        }
    }
}
