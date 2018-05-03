using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class Address
    {
        public string status { get; set; }
        public string info { get; set; }
        public string infocode { get; set; }
        public string count { get; set; }
        public Suggestion suggestion { get; set; }
        public IEnumerable<Districts> districts { get; set; }
    }
}
