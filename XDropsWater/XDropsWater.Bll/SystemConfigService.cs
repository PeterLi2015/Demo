using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll
{
    /// <summary>
    /// 业务逻辑层-系统特殊配置项
    /// </summary>
    public class SystemConfigService : ISystemConfigService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();
        /// <summary>
        /// 判断系统配置项是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsSystemConfigExist(string name)
        {
            Repository<SystemConfigEntity> repo = new Repository<SystemConfigEntity>(uow);
            var sysc = repo.FindBy(p => p.Name == name).FirstOrDefault();
            return sysc != null;
        }

       
    }
}
