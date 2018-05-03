using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Web
{
    public static class ContainerExtensions
    {
        public static void CacheAddresses(this IUnityContainer container)
        {
            var provinceProvider = container.Resolve<IProvince>();
            var AddressDb = container.Resolve<IRepository<AddressEntity>>(
                new ParameterOverride("uow", 
                    new ResolvedParameter<IUnitOfWork>("transient"))
                    );
            var service = container.Resolve<ICacheService>(
                new ParameterOverride("AddressDb", AddressDb)
                );
            var provinceList = provinceProvider.GetAllProvinces();
            AddressModel address = null;
            Address myaddress = null;
            var addresses = new List<Address>();
            foreach(var province in provinceList)
            {
                address = service.GetAddress(province.Province);
                if(address != null)
                {
                    myaddress = JsonConvert.DeserializeObject<Address>(address.JsonValue.Replace("[]", "\"\""));
                    addresses.Add(myaddress);
                }
                
            }
            AddressCache.Addresses = addresses;
        }

        
    }
}