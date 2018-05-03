using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class OrderService : IRegisterObserver
    {
        [Dependency]
        public IUnitOfWork Uow { get; set; }

        [Dependency]
        public IRepository<OrderEntity> OrderDb { get; set; }

        [Dependency]
        public IRepository<ProductEntity> ProductDb { get; set; }

        [Dependency]
        public IRepository<SystemConfigEntity> ConfigDb { get; set; }

        [Dependency]
        public IRepository<MemberRoleEntity> RoldDb { get; set; }

        [Dependency]
        public IRepository<RoleUpgradeEntity> RuDb { get; set; }

        [Dependency]
        public IRepository<OrderDetailsEntity> OdDb { get; set; }

        private Guid userId;

        public void Register(RegisterModel model)
        {
            userId = model.User.Id;
            CreateOrder(model);
        }

        private void CreateOrder(RegisterModel model)
        {
            var total = GetTotalAmount(model);

            var unitAmount = decimal.Parse(ConfigDb.FindBy(o => o.Name == SystemSettingConstants.Price).First().ConfigValue);

            // 省代数量
            var provinceCount = RoldDb.FindBy(o => o.ID < (int)enmMemberRole.Province).Sum(o => o.UpgradeCount);

            // 总代数量
            var generalCount = RoldDb.FindBy(o => o.ID < (int)enmMemberRole.GeneralAgent).Sum(o => o.UpgradeCount);

            // 省代金额
            var provinceAmount = unitAmount * provinceCount;

            // 总代金额
            var generalAmount = unitAmount * generalCount;

            // 以省代级别更新董事
            MemberEntity mem = new MemberEntity();
            mem.CreateBy = model.User.Id;
            mem.CreateOn = DateTime.Now;
            mem.ID = model.Member.MemberId;
            mem.Mobile = model.Member.Mobile;
            
            mem.ParentMemberID = model.Member.ParentId;

            // 之前总金额小于省代，进货之后总金额大于省代但小于总代
            if (total >= provinceAmount && total < generalAmount)
            {
                mem.ProvinceAvailable = 1;
                mem.RoleID = (int)enmMemberRole.Province;
                model.Member.ProvinceAvailable = 1;
                model.Member.RoleId = (int)enmMemberRole.Province;
            }
            else if(total>= generalAmount)
            {
                mem.ProvinceAvailable = 1;
                mem.GeneralAvailable = 1;
                mem.RoleID = (int)enmMemberRole.GeneralAgent;
                model.Member.ProvinceAvailable = 1;
                model.Member.GeneralAvailable = 1;
                model.Member.RoleId = (int)enmMemberRole.GeneralAgent;
            }

            if (mem != null)
            {
                UpdateDirector(new List<MemberEntity>() { mem });
            }

            decimal roleAmount;
            var memberRole = RoldDb.FindBy(mr => mr.ID == (int)enmMemberRole.Customer).FirstOrDefault();
            int iRoleID = Common.GetRoleID1(memberRole.RoleRiseDescription, total, unitAmount, 0, (int)enmMemberRole.Customer, out roleAmount);
            if (iRoleID != (int)enmMemberRole.Customer)
            {
                RoleUpgradeEntity ruEntity = new RoleUpgradeEntity();
                ruEntity.CreateBy = userId;
                ruEntity.CreateOn = DateTime.Now;
                ruEntity.CurrentRoleId = iRoleID;
                ruEntity.OriginalRoleId = (int)enmMemberRole.Customer;
                ruEntity.MemberId = model.Member.MemberId;
                RuDb.Add(ruEntity);
            }
            mem.RoleID = iRoleID;
            model.Member.RoleId = iRoleID;
            mem.CurrentRoleAmount = roleAmount;
            model.Member.CurrentRoleAmount = roleAmount;
            mem.TotalAmount = total;
            model.Member.TotalAmount = total;

            OrderEntity order = new OrderEntity();
            order.SendMemberID = model.Member.ParentId;
            order.Express = "注册";
            order.CreateBy = userId;
            order.CreateOn = DateTime.Now;
            order.FinancialStatus = (int)OrderFinancialStatus.Paid;
            order.ID = Guid.NewGuid();
            order.IsDeliverly = true;
            order.MemberID = model.Member.MemberId;
            order.OrderNo = "DD" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var price = ConfigDb.FindBy(o => o.Name == SystemSettingConstants.Price).Single();
            var totalQuantity = total / decimal.Parse(price.ConfigValue);
            totalQuantity = decimal.Round(totalQuantity, 6);

            decimal totalAmount = 0m;
            order.Description = Common.GetTotalAmountDescription1(memberRole.Price, memberRole.RoleRiseDescription, totalQuantity, memberRole.ID, 0, out totalAmount);
            order.Total = totalAmount;
            order.Quantity = totalQuantity;
            OrderDb.Add(order);

            CreateOrderDetails(model, order.ID);

        }

        private void CreateOrderDetails(RegisterModel model, Guid orderId)
        {
            if (model.Product.Product1Quantity > 0)
            {
                CreateOrderDetails(orderId, 1, model.Product.Product1Quantity);
            }
            if (model.Product.Product2Quantity > 0)
            {
                CreateOrderDetails(orderId, 2, model.Product.Product2Quantity);
            }
            if (model.Product.Product3Quantity > 0)
            {
                CreateOrderDetails(orderId, 3, model.Product.Product3Quantity);
            }
            if (model.Product.Product4Quantity > 0)
            {
                CreateOrderDetails(orderId, 4, model.Product.Product4Quantity);
            }
            if (model.Product.Product5Quantity > 0)
            {
                CreateOrderDetails(orderId, 5, model.Product.Product5Quantity);
            }
            if (model.Product.Product6Quantity > 0)
            {
                CreateOrderDetails(orderId, 6, model.Product.Product6Quantity);
            }
            if (model.Product.Product7Quantity > 0)
            {
                CreateOrderDetails(orderId, 7, model.Product.Product7Quantity);
            }
            if (model.Product.Product8Quantity > 0)
            {
                CreateOrderDetails(orderId, 8, model.Product.Product8Quantity);
            }
            if (model.Product.Product9Quantity > 0)
            {
                CreateOrderDetails(orderId, 9, model.Product.Product9Quantity);
            }
        }

        private void CreateOrderDetails(Guid orderId, int productId, int quantity)
        {
            var entity = new OrderDetailsEntity();
            entity.ID = Guid.NewGuid();
            entity.CreateBy = userId;
            entity.CreateOn = DateTime.Now;
            entity.OrderID = orderId;
            entity.ProductID = 1;
            entity.Quantity = quantity;
            OdDb.Add(entity);
        }

        /// <summary>
        /// 更新董事
        /// </summary>
        /// <param name="memberList"></param>
        public void UpdateDirector(List<MemberEntity> memberList)
        {
            //更新上下级关系表
            UpdateParentChild(memberList);

            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            List<Guid> allParents = new List<Guid>();

            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                foreach (var member in memberList)
                {
                    var parents = db.Database.SqlQuery<Guid>("EXEC [P_GetAllParents] @MemberId", new SqlParameter("@MemberId", member.ID)).ToList();
                    if (parents != null && parents.Any())
                    {
                        allParents.AddRange(parents);
                    }
                }

            }
            Repository<ParentChildEntity> parentRepo = new Repository<ParentChildEntity>(Uow);
            Repository<DirectorEntity> directorRepo = new Repository<DirectorEntity>(Uow);
            if (parentRepo.FindBy(o => allParents.Any(m => m == o.ChildMemberID)).Any())
            {
                var parentList = parentRepo.FindBy(o => allParents.Any(m => m == o.ParentMemberID)).GroupBy(o => o.ParentMemberID);
                MemberEntity member = null;
                int childListCount = 0;
                bool bUpdate = false;
                List<MemberEntity> result = new List<MemberEntity>();
                int count = 0;
                int parentCount = 0;
                int mod = 0;
                foreach (var parent in parentList)
                {
                    parentCount = parent.Count();
                    if (parentCount >= GlobalConstants.ProvinceCountForDirector)
                    {
                        bUpdate = true;
                        if (parentCount == GlobalConstants.ProvinceCountForDirector)
                        {
                            count = 1;
                        }
                        else
                        {
                            //两个总代或三个省代组成一份董事，4个总代或6个省代组成2份董事，同理，3份，4份董事以此类推
                            childListCount = parentRepo.FindBy(o => o.ParentMemberID == parent.Key && o.GeneralAgentCount > 0).Count();
                            count = childListCount / GlobalConstants.GeneralCountForDirector;
                            mod = childListCount % GlobalConstants.GeneralCountForDirector;

                            childListCount = parentRepo.FindBy(o => o.ParentMemberID == parent.Key && o.GeneralAgentCount == 0 && o.ProvinceAgentCount > 0).Count();
                            childListCount += mod;

                            count += childListCount / GlobalConstants.ProvinceCountForDirector;

                        }

                    }
                    else if (parent.Count() == GlobalConstants.GeneralCountForDirector)
                    {
                        childListCount = parentRepo.FindBy(o => o.ParentMemberID == parent.Key && o.GeneralAgentCount > 0).Count();
                        if (childListCount == GlobalConstants.GeneralCountForDirector)
                        {
                            bUpdate = true;
                            count = 1;
                        }
                    }
                    else
                    {
                        bUpdate = false;
                    }
                    if (bUpdate)
                    {
                        if (memberRepo.FindBy(o => o.ID == parent.Key).Any())
                        {
                            if (memberRepo.FindBy(o => o.ID == parent.Key && o.RoleID < (int)enmMemberRole.Director).Any())
                            {
                                member = memberRepo.FindBy(o => o.ID == parent.Key).First();
                                member.GeneralAvailable = 1;
                                member.RoleID = (int)enmMemberRole.Director;
                                member.DirectorDate = DateTime.Now;
                                member.DirectorCount = count;
                                memberRepo.Update(member);
                                result.Add(member);

                                if (!directorRepo.FindBy(o => o.MemberID == member.ID && o.DirectorNo == member.DirectorCount).Any())
                                {
                                    DirectorEntity directorEntity = new DirectorEntity();
                                    directorEntity.ID = Guid.NewGuid();
                                    directorEntity.CreateBy = userId;
                                    directorEntity.CreateOn = DateTime.Now;
                                    directorEntity.DirectorNo = member.DirectorCount;
                                    directorEntity.MemberID = member.ID;
                                    directorRepo.Add(directorEntity);
                                }
                            }
                            else if (memberRepo.FindBy(o => o.ID == parent.Key && o.RoleID == (int)enmMemberRole.Director).Any())
                            {
                                if (!memberRepo.FindBy(o => o.ID == parent.Key && o.RoleID == (int)enmMemberRole.Director && o.DirectorCount >= count).Any())
                                {
                                    member = memberRepo.FindBy(o => o.ID == parent.Key).First();
                                    member.DirectorCount = count;
                                    memberRepo.Update(member);
                                }
                                if (!directorRepo.FindBy(o => o.MemberID == parent.Key && o.DirectorNo == count).Any())
                                {
                                    DirectorEntity directorEntity = new DirectorEntity();
                                    directorEntity.ID = Guid.NewGuid();
                                    directorEntity.CreateBy = userId;
                                    directorEntity.CreateOn = DateTime.Now;
                                    directorEntity.DirectorNo = count;
                                    directorEntity.MemberID = parent.Key;
                                    directorRepo.Add(directorEntity);
                                }
                            }


                        }
                    }
                }
                Uow.Commit();
                if (result.Any())
                {
                    UpdateParentChild(result);
                    UpdateDirector(result);
                }
            }
        }

        /// <summary>
        /// 更新ParentChild
        /// </summary>
        public void UpdateParentChild(List<MemberEntity> memberList)
        {
            foreach (var member in memberList)
            {
                UpdateParentChild(member.ID, member.RoleID);
            }
        }

        /// <summary>
        /// 更新ParentChild表
        /// </summary>
        public void UpdateParentChild(Guid memberId, int roleId)
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_InsertParentChild] @MemberId, @RoleId", new SqlParameter("@MemberId", memberId), new SqlParameter("@RoleId", roleId));
            }
        }

        private decimal GetTotalAmount(RegisterModel model)
        {
            decimal price = 0;
            decimal total = 0;
            if (model.Product.Product1Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 1).First().Price;
                total += model.Product.Product1Quantity * price;
            }
            if (model.Product.Product2Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 2).First().Price;
                total += model.Product.Product2Quantity * price;
            }
            if (model.Product.Product3Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 3).First().Price;
                total += model.Product.Product3Quantity * price;
            }
            if (model.Product.Product4Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 4).First().Price;
                total += model.Product.Product4Quantity * price;
            }
            if (model.Product.Product5Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 5).First().Price;
                total += model.Product.Product5Quantity * price;
            }
            if (model.Product.Product6Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 6).First().Price;
                total += model.Product.Product6Quantity * price;
            }
            if (model.Product.Product7Quantity > 0)
            {
                price = ProductDb.FindBy(o => o.ID == 7).First().Price;
                total += model.Product.Product7Quantity * price;
            }
            return total;
        }
    }
}
