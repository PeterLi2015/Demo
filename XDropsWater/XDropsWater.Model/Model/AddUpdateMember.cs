using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    /// <summary>
    /// a member object for add/update
    /// </summary>
    public class AddUpdateMember : Member
    {
        /// <summary>
        /// product ID
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// identity code
        /// </summary>
        public long IdentityCode { get; set; }
    }
}
