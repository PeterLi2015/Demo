using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface ICacheService
    {
        AddressModel GetAddress(string province);
    }
}
