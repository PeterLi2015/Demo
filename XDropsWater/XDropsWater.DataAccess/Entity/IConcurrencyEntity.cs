using System;

namespace XDropsWater.Dal.DataAccess.Entity
{
    /// <summary>
    /// concurrency checking entity interface
    /// </summary>
    public interface IConcurrencyEntity
    {
        /// <summary>
        /// Using the column for concurrency control
        /// </summary>
        Byte[] RowVersion { get; set; }
    }
}
