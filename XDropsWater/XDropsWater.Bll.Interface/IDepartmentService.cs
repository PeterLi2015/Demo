using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoBang.Model;

namespace DuoBang.Bll.Interface
{
    public interface IDepartmentService : IDataManService
    {
        bool IsDeparmentExist(string name);
    }
}
