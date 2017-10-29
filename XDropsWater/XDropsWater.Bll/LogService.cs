using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.Model;
using XDropsWater.DataAccess;
using XDropsWater.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDropsWater.Bll
{
    public class LogService : ILogService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();
        public void WriteLog(LogModel logModel)
        {
            var repo = new Repository<LogEntity>(uow);
            var log = AutoMapper.Mapper.DynamicMap<LogEntity>(logModel);
            repo.Add(log);
            uow.Commit();
        }
    }
}
