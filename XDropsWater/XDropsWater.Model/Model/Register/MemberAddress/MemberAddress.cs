using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model.Register
{
    public class MemberAddress
    {
        public Province Province { get; set; }
        public City City { get; set; }
        public District District { get; set; }
        public Street Street { get; set; }
        public string Description { get; set; }
    }
}
