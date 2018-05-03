using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model.Register;

namespace XDropsWater.Bll.Interface
{
    public interface IMemberAddressService
    {
        void UpdateAddress(MemberAddress addressModel);
        bool CheckAddressUpdate();
    }
}
