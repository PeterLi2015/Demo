using XDropsWater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Bll.Interface
{
    public interface ILogService
    {
        void WriteLog(LogModel logModel);
    }
}
