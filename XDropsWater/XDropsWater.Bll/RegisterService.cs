using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll
{
    public class RegisterService : BaseService, IRegisterService
    {
        private readonly IUnitOfWork Uow;

        [Dependency]
        public IRepository<AddressEntity> AddressDb { get; set; }

        [Dependency]
        public IRepository<MemberEntity> MemDb { get; set; }

        [Dependency]
        public IRepository<UserEntity> UserDb { get; set; }
        
        [Dependency]
        public IRegisterObserver[] Observers { get; set; }

        public RegisterService(IUnitOfWork Uow)
        {
            this.Uow = Uow;
        }

        public void Register(RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Member.Mobile))
            {
                throw new Exception("手机号码不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.Member.MemberName))
            {
                throw new Exception("代理姓名不能为空");
            }
            if (string.IsNullOrWhiteSpace(model.Member.ParentMobile))
            {
                throw new Exception("上级手机号码不能为空");
            }
            if (string.Equals(model.Member.Mobile, model.Member.ParentMobile))
            {
                throw new Exception("上级手机号码不能和本人手机号码相同");
            }

            var total = model.Product.Product1Quantity +
                model.Product.Product2Quantity +
                model.Product.Product3Quantity +
                model.Product.Product4Quantity +
                model.Product.Product5Quantity +
                model.Product.Product6Quantity +
                model.Product.Product7Quantity +
                model.Product.Product8Quantity +
                model.Product.Product9Quantity +
                model.Product.Product10Quantity +
                model.Product.Product11Quantity +
                model.Product.Product12Quantity;
            if (total == 0)
            {
                throw new Exception("必须至少购买一件商品");
            }

            if (!UserDb.FindBy(o => o.Account == model.User.UserName && o.Password == o.SwitchEncryptDecrypt(model.User.Password)).Any())
            {
                throw new Exception("用户名或密码错误");
            }
            else
            {
                model.User.Id = UserDb.FindBy(o => o.Account == model.User.UserName && o.Password == o.SwitchEncryptDecrypt(model.User.Password)).First().ID;
            }

            if (!MemDb.FindBy(o => o.Mobile == model.Member.ParentMobile).Any())
            {
                throw new Exception("上级手机号码不存在");
            }
            else
            {
                model.Member.ParentId = MemDb.FindBy(o => o.Mobile == model.Member.ParentMobile).First().ID;
            }

            if (MemDb.FindBy(o => o.Mobile == model.Member.Mobile).Any())
            {
                throw new Exception("手机号码已经被注册了");
            }

            #region Check if IdentityNo has beeen used
            if (!string.IsNullOrWhiteSpace(model.Member.IdentityNo))
            {
                if (MemDb.FindBy(m => string.Equals(m.IdentityNo, model.Member.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经被注册了");
                }
            }
            #endregion

            model.Member.MemberId = Guid.NewGuid();

            Observers.ToList().ForEach(o =>
            {
                o.Register(model);
            });

            Uow.Commit();
        }

        public void RegisterProduct(IEnumerable<ProductRegister> productList)
        {
            throw new NotImplementedException();
        }

        public void RegisterUser(Member user)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddress(AddressModel address)
        {
            if (AddressDb.FindBy(o => o.Province == address.Province).Any())
            {
                var entity = AddressDb.FindBy(o => o.Province == address.Province).First();
                entity.JsonValue = address.JsonValue;
                entity.UpdateBy = this.CurrentUser.ID;
                entity.UpdateOn = DateTime.Now;
                AddressDb.Update(entity);
            }
            else
            {
                var entity = new AddressEntity();
                entity.ID = Guid.NewGuid();
                entity.CreateOn = DateTime.Now;
                entity.CreateBy = this.CurrentUser.ID;
                entity.UpdateBy = this.CurrentUser.ID;
                entity.UpdateOn = DateTime.Now;
                entity.JsonValue = address.JsonValue;
                entity.Province = address.Province;
                entity.ProvinceCode = address.ProvinceCode;
                AddressDb.Add(entity);
            }
            Uow.Commit();
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

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Province> GetProvinces()
        {
            var provinces = AddressDb.GetAll();
            var result = new List<Province>();
            Province province = null;
            provinces.OrderBy(o => o.ProvinceCode).ToList().ForEach(o =>
            {
                province = new Province();
                province.ProvinceCode = o.ProvinceCode;
                province.ProvinceName = o.Province;
                result.Add(province);
            });
            return result;
        }


    }
}
