using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// Entity Status
    /// </summary>
    public static class DeleteStatus
    {
        /// <summary>
        /// DEFAULT Status
        /// </summary>
        public const int Normal = 0;

        /// <summary>
        /// Delete Status
        /// </summary>
        public const int Deleted = 1;
    }

    /// <summary>
    /// Interface IEntityStatus
    /// </summary>
    public interface IDeleteStatus
    {
        /// <summary>
        /// Entity Status
        /// </summary>
        int EntityStatus
        {
            get;
            set;
        }
    }
}
