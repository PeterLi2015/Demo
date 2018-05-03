using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;
using XDropsWater.Model.Register;

namespace XDropsWater.Bll
{
    public class MemberAddressService : BaseService, IRegisterObserver, IMemberAddressService
    {
        private readonly IRepository<MemberAddressEntity> AddressDb;
        private readonly IUnitOfWork uow;
        private readonly IRepository<MemberEntity> memDb;

        public MemberAddressService(IRepository<MemberAddressEntity> addressDb, IUnitOfWork uow, IRepository<MemberEntity> memDb)
        {
            this.AddressDb = addressDb;
            this.uow = uow;
            this.memDb = memDb;
        }

        public bool CheckAddressUpdate()
        {
            bool result = true;
            if (this.CurrentUser.RoleID >= (int)enmMemberRole.Customer)
            {
                if (!AddressDb.FindBy(o => o.MemberID == this.CurrentUser.MemberID).Any())
                {
                    result = false;
                }
            }
            
            return result;
        }

        public void UpdateAddress(MemberAddress addressModel)
        {
            MemberAddressEntity entity = new MemberAddressEntity();
            entity.CityCode = addressModel.City.CityCode;
            entity.CityName = addressModel.City.CityName;
            entity.CreateBy = this.CurrentUser.ID;
            entity.CreateOn = DateTime.Now;
            entity.Description = addressModel.Description;
            entity.DistrictCode = addressModel.District.DistrictCode;
            entity.DistrictName = addressModel.District.DistrictName;
            entity.ID = Guid.NewGuid();
            entity.MemberID = this.CurrentUser.MemberID;
            entity.ProvinceCode = addressModel.Province.ProvinceCode;
            entity.ProvinceName = addressModel.Province.ProvinceName;
            entity.StreetCode = addressModel.Street.StreetCode;
            entity.StreetName = addressModel.Street.StreetName;
            AddressDb.Add(entity);
            var memEntity = memDb.FindBy(o => o.ID == this.CurrentUser.MemberID).First();
            memEntity.Address = GetAddress(addressModel);
            memDb.Update(memEntity);
            uow.Commit();
        }
        private string GetAddress(MemberAddress address)
        {
            return address.Province.ProvinceName +
                address.City.CityName +
                address.District.DistrictName +
                address.Street.StreetName +
                address.Description;
        }

        public void Register(RegisterModel model)
        {
            MemberAddressEntity entity = new MemberAddressEntity();
            entity.CityCode = model.MemberAddress.City.CityCode;
            entity.CityName = model.MemberAddress.City.CityName;
            entity.CreateBy = model.User.Id;
            entity.CreateOn = DateTime.Now;
            entity.Description = model.MemberAddress.Description;
            entity.DistrictCode = model.MemberAddress.District.DistrictCode;
            entity.DistrictName = model.MemberAddress.District.DistrictName;
            entity.ID = Guid.NewGuid();
            entity.MemberID = model.Member.MemberId;
            entity.ProvinceCode = model.MemberAddress.Province.ProvinceCode;
            entity.ProvinceName = model.MemberAddress.Province.ProvinceName;
            entity.StreetCode = model.MemberAddress.Street.StreetCode;
            entity.StreetName = model.MemberAddress.Street.StreetName;
            AddressDb.Add(entity);
        }
    }
}
