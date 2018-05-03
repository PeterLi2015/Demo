using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class Districts
    {
        public string citycode { get; set; }
        public string adcode { get; set; }
        public string name { get; set; }
        public string center { get; set; }
        public string level { get; set; }
        public IEnumerable<Districts> districts { get; set; }
    }
}
