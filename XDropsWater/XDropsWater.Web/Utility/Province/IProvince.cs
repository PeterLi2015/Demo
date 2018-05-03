using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDropsWater.Model;

namespace XDropsWater.Web
{
    public interface IProvince
    {
        IEnumerable<AddressModel> GetAllProvinces();
    }
}