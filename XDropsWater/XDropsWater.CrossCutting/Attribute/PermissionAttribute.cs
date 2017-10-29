using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.Attribute
{
    public class PermissionAttribute : System.Attribute
    {
        public string Permission
        {
            get;
            set;
        }
    }
}
