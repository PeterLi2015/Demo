using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll
{
    public class CacheService : ICacheService
    {
        private readonly IRepository<AddressEntity> AddressDb;

        public CacheService(IRepository<AddressEntity> AddressDb)
        {
            this.AddressDb = AddressDb;
        }

        public AddressModel GetAddress(string province)
        {
            AddressModel result = null;
            if (AddressDb.FindBy(o => o.Province == province).Any())
            {
                var entity = AddressDb.FindBy(o => o.Province == province).First();
                Mapper.CreateMap<AddressEntity, AddressModel>();
                result = Mapper.Map<AddressModel>(entity);
            }
            return result;
        }
    }
}
