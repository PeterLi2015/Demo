using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.DataAccess
{
    /// <summary>
    /// // Summary:
    ///         Defines the different ways to handle modified properties when resolving
    ///         consurrency.
    /// </summary>
    public enum ResolveConcurrencyMode
    {
        /// <summary>
        /// Discard all changes on the client and refresh values with store values. 
        /// </summary>
        StoreWins = 1,
        /// <summary>
        /// keeping all values on client object.
        /// </summary>
        ClientWins = 2,
        /// <summary>
        /// User Resolve Concurrency
        /// </summary>
        UserResolveConcurrency = 3,
    }
}
