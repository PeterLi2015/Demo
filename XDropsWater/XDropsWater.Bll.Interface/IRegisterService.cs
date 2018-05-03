using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface IRegisterService
    {
        void Register(RegisterModel register);
        void RegisterUser(Member user);
        void RegisterProduct(IEnumerable<ProductRegister> productList);
        void UpdateAddress(AddressModel address);
        AddressModel GetAddress(string province);
    }
}
