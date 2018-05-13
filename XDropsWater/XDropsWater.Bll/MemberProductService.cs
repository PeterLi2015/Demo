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
    public class MemberProductService : IRegisterObserver
    {
        private readonly IUnitOfWork Uow;

        private readonly IRepository<MemberProductEntity> MpDb;

        private readonly IRepository<ProductEntity> ProductDb;

        public MemberProductService(IUnitOfWork Uow, IRepository<MemberProductEntity> MpDb, IRepository<ProductEntity> ProductDb)
        {
            this.Uow = Uow;
            this.MpDb = MpDb;
            this.ProductDb = ProductDb;
        }

        public void Register(RegisterModel model)
        {
            CheckParentQuantity(model);
            ParentSubQuantity(model);
            MemberAddQuantity(model);
        }

        private void MemberAddQuantity(RegisterModel model)
        {
            var memId = model.Member.MemberId;
            var userId = model.User.Id;
            if (model.Product.Product1Quantity > 0)
            {
                MemberAddQuantity(1, model.Product.Product1Quantity, memId, userId);
            }
            if (model.Product.Product2Quantity > 0)
            {
                MemberAddQuantity(2, model.Product.Product2Quantity, memId, userId);
            }
            if (model.Product.Product3Quantity > 0)
            {
                MemberAddQuantity(3, model.Product.Product3Quantity, memId, userId);
            }
            if (model.Product.Product4Quantity > 0)
            {
                MemberAddQuantity(4, model.Product.Product4Quantity, memId, userId);
            }
            if (model.Product.Product5Quantity > 0)
            {
                MemberAddQuantity(5, model.Product.Product5Quantity, memId, userId);
            }
            if (model.Product.Product6Quantity > 0)
            {
                MemberAddQuantity(6, model.Product.Product6Quantity, memId, userId);
            }
            if (model.Product.Product7Quantity > 0)
            {
                MemberAddQuantity(7, model.Product.Product7Quantity, memId, userId);
            }
        }

        private void MemberAddQuantity(int productId, int quantity, Guid memberId, Guid curUserId)
        {
            // 购买代理加库存
            if (MpDb.FindBy(o => o.MemberID == memberId && o.ProductID == productId).Any())
            {
                var buyMp = MpDb.FindBy(o => o.MemberID == memberId && o.ProductID == productId).First();
                buyMp.Quantity += quantity;
                MpDb.Update(buyMp);
            }
            else
            {
                var buyMp = new MemberProductEntity();
                buyMp.CreateBy = curUserId;
                buyMp.CreateOn = DateTime.Now;
                buyMp.ID = Guid.NewGuid();
                buyMp.MemberID = memberId;
                buyMp.ProductID = productId;
                buyMp.Quantity = quantity;
                MpDb.Add(buyMp);
            }
        }

        private void ParentSubQuantity(RegisterModel model)
        {
            if (model.Product.Product1Quantity > 0)
            {
                ParentSubQuantity(1, model.Member.ParentId, model.Product.Product1Quantity);
            }
            if (model.Product.Product2Quantity > 0)
            {
                ParentSubQuantity(2, model.Member.ParentId, model.Product.Product2Quantity);
            }
            if (model.Product.Product3Quantity > 0)
            {
                ParentSubQuantity(3, model.Member.ParentId, model.Product.Product3Quantity);
            }
            if (model.Product.Product4Quantity > 0)
            {
                ParentSubQuantity(4, model.Member.ParentId, model.Product.Product4Quantity);
            }
            if (model.Product.Product5Quantity > 0)
            {
                ParentSubQuantity(5, model.Member.ParentId, model.Product.Product5Quantity);
            }
            if (model.Product.Product6Quantity > 0)
            {
                ParentSubQuantity(6, model.Member.ParentId, model.Product.Product6Quantity);
            }
            if (model.Product.Product7Quantity > 0)
            {
                ParentSubQuantity(7, model.Member.ParentId, model.Product.Product7Quantity);
            }
        }

        private void ParentSubQuantity(int productId, Guid parentId, int quantity)
        {
            // 发货代理减库存
            var sendMp = MpDb.FindBy(o => o.ProductID == productId && o.MemberID == parentId).First();
            sendMp.Quantity -= quantity;
            MpDb.Update(sendMp);
        }

        private void CheckParentQuantity(RegisterModel model)
        {
            if (model.Product.Product1Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 1, model.Member.ParentId);
            }
            if (model.Product.Product2Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 2, model.Member.ParentId);
            }
            if (model.Product.Product3Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 3, model.Member.ParentId);
            }
            if (model.Product.Product4Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 4, model.Member.ParentId);
            }
            if (model.Product.Product5Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 5, model.Member.ParentId);
            }
            if (model.Product.Product6Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 6, model.Member.ParentId);
            }
            if (model.Product.Product7Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 7, model.Member.ParentId);
            }
            if (model.Product.Product8Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 8, model.Member.ParentId);
            }
            if (model.Product.Product9Quantity > 0)
            {
                CheckParentStore(model.Product.Product1Quantity, 9, model.Member.ParentId);
            }
        }

        private void CheckParentStore(int quantity, int productId, Guid parentId)
        {
            MemberProductEntity sendMp = null;
            if (!MpDb.FindBy(o => o.ProductID == productId && o.MemberID == parentId && o.Quantity >= quantity).Any())
            {
                var product = ProductDb.FindBy(o => o.ID == productId).First();
                sendMp = MpDb.FindBy(o => o.ProductID == productId && o.MemberID == parentId).First();
                int storeQuantity = 0;
                if (sendMp != null)
                {
                    storeQuantity = sendMp.Quantity;
                }
                throw new Exception(string.Format("上级【{0}】库存只有{1}件，少于{2}件，不能注册", product.Name, storeQuantity, quantity));
            }
        }
    }
}
