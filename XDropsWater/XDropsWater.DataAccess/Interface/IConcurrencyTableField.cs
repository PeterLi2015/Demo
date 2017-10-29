using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// Concurrency Field for Whole Table
    /// </summary>
    interface IConcurrencyTableField
    {
        /// <summary>
        /// Add TimeStamp Field to make a concurrency if any field of table changed.
        /// </summary>
        [Timestamp]
        [ConcurrencyCheck]
        byte[] TimeStamp { get; set; }
    }
}
