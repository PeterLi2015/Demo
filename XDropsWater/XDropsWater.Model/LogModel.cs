using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class LogModel
    {
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string ErrMsg { get; set; }
        public Guid ID { get; set; }
        public string ModuleName { get; set; }
    }
}
