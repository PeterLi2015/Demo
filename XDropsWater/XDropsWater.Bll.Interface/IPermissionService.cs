using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoBang.Model;

namespace DuoBang.Bll.Interface
{
    public interface IPermissionService : IDataManService
    {
        bool IsPermissionExist(string name);
    }
}
