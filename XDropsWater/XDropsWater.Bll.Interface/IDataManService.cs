using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface IDataManService : IService
    {
        List<object> GetAll();

        object Get(Guid id);

        ErrorCodes AddOrUpdate(object model);

        ErrorCodes Delete(Guid id);
    }
}
