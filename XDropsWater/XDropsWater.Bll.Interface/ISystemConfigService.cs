using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface ISystemConfigService
    {
        bool IsSystemConfigExist(string name);
        //object GetGroupID();
        //ErrorCodes Update();
    }
}
