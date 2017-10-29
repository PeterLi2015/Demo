using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HideValueAttribute : System.Attribute
    {
        public object Value
        {
            get;
            set;
        }
    }
}
