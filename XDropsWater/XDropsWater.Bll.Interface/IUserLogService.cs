using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface IUserLogService : IService
    {
        List<UserLog> Search(DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, out int total);
        void AddLog(UserOperations op, params object[] paramenters);
    }
}
