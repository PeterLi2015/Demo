using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDropsWater.Model;

namespace XDropsWater.Web
{
    public class AddressCache
    {
        public static IEnumerable<Address> Addresses
        {
            get;set;
        }
        public static IEnumerable<Districts> AllProvinces
        {
            get;set;
        }
    }
}