using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.Model;
using XDropsWater.CrossCutting.IDCard;
using XDropsWater.DataAccess;
using XDropsWater.DataAccess.Interface;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Configuration;
using AutoMapper;
using System.Text;
using Unity.Attributes;

namespace XDropsWater.Bll
{
    public class MemberService : BaseService, IMemberService, IRegisterObserver
    {
        [Dependency]
        public IUnitOfWork Uow { get; set; }

        [Dependency]
        public IRepository<MemberEntity> MemDb { get; set; }

        [Dependency]
        public IRepository<ShoppingCartEntity> shoppingCartDb { get; set; }

        [Dependency]
        public IRepository<ProductEntity> ProductDb { get; set; }

        /// <summary>
        /// 添加不存在的代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ErrorCodes AddNoExistsMember(MemberModel model)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var member = repo.FindBy(p => p.Mobile == model.Mobile).FirstOrDefault();
            if (member == null)
            {
                Add(model);
            }
            return ErrorCodes.Successed;
        }
        public ErrorCodes Add(MemberModel model)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            if (!IDCardUtil.CheckIDCard(model.IdentityCardNo))
            {
                return ErrorCodes.IDCardError;
            }
            MemberEntity member = new MemberEntity();
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = DateTime.Now;
            //member.IdentityCardNo = model.IdentityCardNo;
            member.MemberName = model.MemberName;
            member.Mobile = model.Mobile;
            repo.Add(member);
            Uow.Commit();
            return ErrorCodes.Successed;
        }

        public MemberModel Get(string mobile)
        {
            MemberModel m = new MemberModel();
            m.MemberName = "";

            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var mem = repo.FindBy(p => p.Mobile == mobile).FirstOrDefault();
            if (mem == null)
            {
                m.Result = new OpResult();
                m.Result.ErrCode = ErrorCodes.MemberNotExist;
                return m;
            }
            m.MemberName = mem.MemberName;
            return m;
        }

        /// <summary>
        /// 根据电话号码获取代理对象
        /// </summary>
        /// <param name="mobile">电话号码</param>
        /// <returns>代理对象</returns>
        public Member GetMemberByMobile(string mobile)
        {
            var db = new Repository<MemberEntity>(Uow);
            var entity = db.FindBy(p => p.Mobile == mobile).FirstOrDefault();
            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            return Mapper.Map<Member>(entity);
        }

        public List<MemberModel> Get(string idCardNo, string name, ref int total, int page = 1, int rows = 10)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var list = repo.FindBy(p => p.CreateBy == this.CurrentUser.ID);
            var all = list.ToList();
            if (!string.IsNullOrWhiteSpace(idCardNo))
            {
                list = list.Where(p => p.Mobile.Contains(idCardNo));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                list = list.Where(p => p.MemberName.Contains(name));
            }

            //var list = repo.FindBy(p => p.CreateBy == this.CurrentUser.ID && p.IdentityCardNo.Contains(idCardNo) && p.MemberName.Contains(name)).ToList();
            var modelList = new List<MemberModel>();
            MemberModel model = null;
            MemberEntity entity = null;

            total = list.Count();
            var members = list.OrderBy(p => p.MemberName).Skip((page - 1) * rows).Take(rows).ToList();
            if (members != null && members.Any())
            {
                foreach (var item in members)
                {
                    entity = all.Where(p => p.ID == item.ParentMemberID).SingleOrDefault();
                    model = new MemberModel();
                    //model.BankCardNo = item.BankCardNo;
                    //model.BankName = item.BankName;
                    model.CreateBy = item.CreateBy == null ? new Guid("00000000-0000-0000-0000-000000000000") : item.CreateBy.Value;
                    model.CreateOn = item.CreateOn == null ? new DateTime(1900, 1, 1) : item.CreateOn.Value;
                    model.ID = item.ID;
                    //model.IdentityCardNo = item.IdentityCardNo;
                    model.MemberName = item.MemberName;
                    //model.ParentIdentityCardNo = entity != null ? entity.IdentityCardNo : "";
                    if (item.ParentMemberID.HasValue)
                        model.ParentMemberID = item.ParentMemberID.Value;
                    model.UpdateBy = item.UpdateBy == null ? new Guid("00000000-0000-0000-0000-000000000000") : item.UpdateBy.Value;
                    model.UpdateOn = item.UpdateOn == null ? new DateTime(1900, 1, 1) : item.UpdateOn.Value;
                    model.ParentName = entity != null ? entity.MemberName : "";
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        public List<MemberModel> GetDirectChildMember(string mobileOrName)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var list = repo.FindBy(m => m.ParentMemberID == this.CurrentUser.MemberID);
            if (!string.IsNullOrWhiteSpace(mobileOrName))
                list = list.Where(p => p.Mobile.Contains(mobileOrName) || p.MemberName.Contains(mobileOrName));

            var modelList = new List<MemberModel>();
            MemberModel model = null;

            var members = list.OrderByDescending(p => p.CreateOn).ToList();

            if (members != null && members.Any())
            {
                MemberEntity parentEntity = repo.FindBy(m => m.ID == this.CurrentUser.MemberID).FirstOrDefault();
                foreach (var item in members)
                {
                    model = new MemberModel();
                    model.ID = item.ID;
                    model.Mobile = item.Mobile;
                    model.MemberName = item.MemberName;
                    model.ParentName = parentEntity.MemberName;
                    model.ParentMemberMobile = parentEntity.Mobile;
                    model.MemberRoleID = item.RoleID;
                    model.Address = item.Address;
                    if (item.CreateOn.HasValue)
                        model.CreateOn = item.CreateOn.Value;
                    model.IdentityNo = item.IdentityNo;
                    modelList.Add(model);
                }
            }
            return modelList;
        }


        /// <summary>
        /// 获取我添加的和我直接下属的代理
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页显示行数</param>
        /// <param name="mobileOrName">查询条件：手机号码或姓名</param>
        /// <param name="isDelivery">查询条件：是否发货</param>
        /// <param name="orderLevel">订单级别：公司发货的订单，代理发货的订单</param>
        /// <returns></returns>
        public MemberSummary GetDirectChildMember1(int page, int size, string mobileOrName)
        {
            var result = new MemberSummary();
            var db = new Repository<MemberEntity>(Uow);

            Expression<Func<MemberEntity, bool>> whereExp = o => o.ID != Guid.Empty && o.ParentMemberID == this.CurrentUser.MemberID;
            whereExp = whereExp.Or(o => o.CreateBy == this.CurrentUser.ID);
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                whereExp = whereExp.And(o => (o.Mobile.Contains(mobileOrName)
                || o.MemberName.ToUpper().Contains(mobileOrName.ToUpper()))
                );
            }
            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
            var members = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * size).Take(size).ToList();
            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<IdentityCodeEntity, IdentityCode>();
            Mapper.CreateMap<MemberEntity, Member>();

            result.Members = Mapper.Map<IEnumerable<Member>>(members);
            CalculateRowNo(result, result.Members, page, size, totalCount);

            return result;

        }


        /// <summary>
        /// 获取新订单数量
        /// </summary>
        /// <returns></returns>
        public int GetNewOrderCount()
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);

            IQueryable<OrderEntity> orderList = null;

            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                orderList = orderRepo.FindBy(o => o.SendMemberID == this.CurrentUser.MemberID && !o.IsDeliverly && (o.Status != (int)OrderStatus.LessAmount));
            }
            else if (this.CurrentUser.UserRoleID == (int)enmRoles.Admin ||
                this.CurrentUser.UserRoleID == (int)enmRoles.All)
            {
                orderList = orderRepo.FindBy(o => o.SendMemberID == Guid.Empty && !o.IsDeliverly && (o.Status != (int)OrderStatus.LessAmount));
            }
            else if (this.CurrentUser.UserRoleID == (int)enmRoles.Financial)
            {
                orderList = orderRepo.FindBy(o => o.SendMemberID == Guid.Empty && !o.IsDeliverly && (o.FinancialStatus != (int)OrderFinancialStatus.Paid));
            }
            return orderList.Count();
        }

        public int GetSelfNewOrderCount()
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);

            IQueryable<OrderEntity> orderList = null;

            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                orderList = orderRepo.FindBy(o => o.MemberID == this.CurrentUser.MemberID && !o.IsDeliverly);
            }
            return orderList == null ? 0 : orderList.Count();
        }

        public int GetMemberCount()
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            int iRet = 0;
            if (this.CurrentUser.UserRoleID == (int)enmRoles.Admin ||
                this.CurrentUser.UserRoleID == (int)enmRoles.All)
            {
                iRet = repo.FindBy(o => o.RoleID > 0).Count();
            }
            else if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                iRet = repo.FindBy(o => o.RoleID > 0 && o.ParentMemberID == this.CurrentUser.ID).Count();
            }
            return iRet;
        }

        /// <summary>
        /// 发货记录
        /// </summary>
        /// <returns></returns>
        public int GetNewExpress()
        {
            var db = new Repository<ExpressEntity>(Uow);
            int iRet = 0;
            if (this.CurrentUser.UserRoleID == (int)enmRoles.Admin ||
                this.CurrentUser.UserRoleID == (int)enmRoles.All)
            {
                iRet = db.FindBy(o => o.Status == 0).Count();
            }
            else if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                iRet = db.FindBy(o => o.Status == 0 && o.MemberID == this.CurrentUser.MemberID).Count();
            }
            return iRet;
        }

        /// <summary>
        /// 获取高号代理信息
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="idCardNo"></param>
        /// <param name="hasChecked"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<MemberModel> GetMember(string search)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var list = repo.FindBy(o => o.RoleID > 0);
            if (!string.IsNullOrWhiteSpace(search))
                list = list.Where(p => p.Mobile.Contains(search) || p.MemberName.ToUpper().Contains(search.ToUpper()));

            //var list = repo.FindBy(p => p.CreateBy == createBy && p.IsNew == true);
            var allMembers = repo.GetAll().ToList();


            //var list = repo.FindBy(p => p.CreateBy == this.CurrentUser.ID && p.IdentityCardNo.Contains(idCardNo) && p.MemberName.Contains(name)).ToList();
            var modelList = new List<MemberModel>();
            MemberModel model = null;

            var members = list.OrderByDescending(p => p.CreateOn).ToList();

            if (members != null && members.Any())
            {
                MemberEntity parentEntity = null;
                foreach (var item in members)
                {
                    parentEntity = allMembers.Where(p => p.ID == item.ParentMemberID).SingleOrDefault();
                    model = new MemberModel();
                    //model.BankCardNo = item.BankCardNo;
                    //model.BankName = item.BankName;
                    //model.CreateBy = item.CreateBy == null ? new Guid("00000000-0000-0000-0000-000000000000") : item.CreateBy.Value;
                    //model.CreateOn = item.CreateOn == null ? new DateTime(1900, 1, 1) : item.CreateOn.Value;
                    model.ID = item.ID;
                    model.Mobile = item.Mobile;
                    //model.IdentityCardPath = item.IdentityCardPath;
                    //model.IsNew = item.IsNew;
                    model.MemberName = item.MemberName;

                    model.ParentName = parentEntity != null ? parentEntity.MemberName : "";
                    model.ParentMemberMobile = parentEntity != null ? parentEntity.Mobile : "";
                    model.MemberRoleID = item.RoleID;
                    model.Address = item.Address;
                    model.CreateOn = item.CreateOn.HasValue ? item.CreateOn.Value : default(DateTime);
                    model.IdentityNo = item.IdentityNo;
                    model.CurrentRoleQuantity = item.CurrentRoleQuantity;
                    model.TotalQuantity = item.TotalQuantity;
                    model.ProvinceAvailable = item.ProvinceAvailable;
                    model.GeneralAvailable = item.GeneralAvailable;

                    modelList.Add(model);
                }
            }
            return modelList;
        }


        public MemberSummary GetMember1(int page, int size, string search)
        {
            var result = new MemberSummary();

            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            Expression<Func<MemberEntity, bool>> whereExp = o => o.ID != Guid.Empty && o.RoleID > 0;
            if (!string.IsNullOrWhiteSpace(search))
            {
                whereExp = whereExp.And(o => o.Mobile.Contains(search) || o.MemberName.ToUpper().Contains(search.ToUpper()));
            }
            var totalCount = repo.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);

            var members = repo.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * size).Take(size).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();

            result.Members = Mapper.Map<IEnumerable<Member>>(members);
            CalculateRowNo(result, result.Members, page, size, totalCount);
            return result;
        }

        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="page"></param>
        /// <param name="siez"></param>
        /// <param name="orderDetailsId">订单明细ID</param>
        /// <returns></returns>
        public IdentityCodeSummary GetCodePages(int page, int size, Guid orderDetailsId)
        {
            var result = new IdentityCodeSummary();
            var db = new Repository<IdentityCodeEntity>(Uow);
            Expression<Func<IdentityCodeEntity, bool>> whereExp = o => o.ID != Guid.Empty && o.OrderDetailsID == orderDetailsId;
            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
            var items = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * size).Take(size).ToList();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();
            Mapper.CreateMap<OrderDetailsEntity, OrderDetails>();
            Mapper.CreateMap<IdentityCodeEntity, IdentityCode>();
            result.IdentityCodes = Mapper.Map<IEnumerable<IdentityCode>>(items);
            CalculateRowNo(result, result.IdentityCodes, page, size, totalCount);
            return result;
        }

        public int GetMembersCount()
        {
            var repo = new Repository<MemberEntity>(Uow);
            return repo.FindBy(o => o.RoleID > 0).Count();
        }

        /// <summary>
        /// 获取代理订单
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="idCardNo"></param>
        /// <param name="hasChecked"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<MemberOrderModel> GetMemberOrder(string mobileOrName, bool? isDelivery)
        {
            List<MemberOrderModel> list = new List<MemberOrderModel>();
            MemberOrderModel model = null;
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);

            List<MemberEntity> memberList = null;
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                memberList = memberRepo.FindBy(m => m.Mobile == mobileOrName || m.MemberName.ToUpper().Contains(mobileOrName.ToUpper())).ToList();

                if (memberList == null)
                {
                    return list;
                }
            }
            else
            {
                if (this.CurrentUser.UserRoleID == (int)enmRoles.Admin ||
                    this.CurrentUser.UserRoleID == (int)enmRoles.All)
                    memberList = memberRepo.GetAll().ToList();
                else if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
                {
                    var curUserMemberRoleId = memberRepo.FindBy(o => o.ID == this.CurrentUser.MemberID).First().RoleID;
                    memberList = memberRepo.FindBy(o => o.RoleID <= curUserMemberRoleId).ToList();
                }
            }

            var orderList = orderRepo.FindBy(o => memberList.Select(m => m.ID).Contains(o.MemberID)).OrderByDescending(o => o.CreateOn);
            if (isDelivery.HasValue)
            {
                orderList = orderList.Where(o => o.IsDeliverly == isDelivery.Value).OrderByDescending(o => o.CreateOn);
            }
            //if (this.CurrentUser.MemberId != null && this.CurrentUser.MemberId != Guid.Empty)
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                orderList = orderList.Where(o => o.SendMemberID == this.CurrentUser.MemberID).OrderByDescending(o => o.CreateOn);
            }
            else if (this.CurrentUser.UserRoleID == (int)enmRoles.Admin ||
                this.CurrentUser.UserRoleID == (int)enmRoles.All)
            {
                orderList = orderList.Where(o => o.SendMemberID == Guid.Empty).OrderByDescending(o => o.CreateOn);
            }
            foreach (OrderEntity order in orderList.ToList() ?? Enumerable.Empty<OrderEntity>())
            {
                model = new MemberOrderModel();
                model.Address = memberList.First(o => o.ID == order.MemberID).Address;
                model.CreateOn = order.CreateOn.Value;
                model.ID = order.ID;
                model.IsSubmit = order.IsDeliverly;
                model.MemberName = memberList.First(o => o.ID == order.MemberID).MemberName;
                model.Mobile = memberList.First(o => o.ID == order.MemberID).Mobile;
                model.Quantity = order.Quantity;
                model.MemberRoleID = memberList.First(o => o.ID == order.MemberID).RoleID;
                model.Description = order.Description;
                model.Total = order.Total;
                model.Express = order.Express;
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 删除编号
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveCode(Guid Id)
        {
            var db = new Repository<IdentityCodeEntity>(Uow);
            var entity = db.FindBy(o => o.ID == Id).First();
            var code = entity.Code;
            var productId = entity.OrderDetails.ProductID;

            //还原识别码
            var oriCode = db.FindBy(o => o.Code == code && o.OrderDetails.ProductID == productId && o.Status == (int)CodeStatus.ChildNotSend).FirstOrDefault();
            if (oriCode != null)
            {
                oriCode.Status = (int)CodeStatus.Available;
                db.Update(oriCode);
            }

            //设置订单明细识别码未填满
            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var od = odDb.FindBy(o => o.ID == entity.OrderDetailsID).First();
            od.Status = (int)OrderDetailsStatus.CodeNotFull;
            odDb.Update(od);

            //设置订单识别码未填满
            var orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == entity.OrderDetails.OrderID).First();
            order.Status = (int)OrderStatus.CodeNotFull;
            orderDb.Update(order);

            db.Remove(entity);
            Uow.Commit();
        }

        /// <summary>
        /// 获取代理订单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">每页显示行数</param>
        /// <param name="mobileOrName">查询条件：手机号码或姓名</param>
        /// <param name="isDelivery">查询条件：是否发货</param>
        /// <param name="orderLevel">订单级别：公司发货的订单，代理发货的订单</param>
        /// <returns></returns>
        public OrderSummary GetMemberOrder1(int page, int size, string mobileOrName, bool? isDelivery, enmOrderLevel orderLevel)
        {
            var result = new OrderSummary();
            var repo = new Repository<OrderEntity>(Uow);
            Expression<Func<OrderEntity, bool>> whereExp = o => o.ID != Guid.Empty;
            whereExp = whereExp.And(o => (o.Status != (int)OrderStatus.LessAmount));
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                whereExp = whereExp.And(o => (o.Member.Mobile.Contains(mobileOrName)
                || o.Member.MemberName.ToUpper().Contains(mobileOrName.ToUpper()))
                );
            }
            if (isDelivery.HasValue)
            {
                whereExp = whereExp.And(o => o.IsDeliverly == isDelivery.Value);
            }
            switch (orderLevel)
            {
                case enmOrderLevel.Agency:
                    {
                        whereExp = whereExp.And(o => o.SendMemberID == this.CurrentUser.MemberID);
                    }
                    break;
                case enmOrderLevel.Company:
                    {
                        whereExp = whereExp.And(o => o.SendMemberID == Guid.Empty);
                    }
                    break;
                default:
                    break;
            }

            var totalCount = repo.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
            var orders = repo.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * size).Take(size).ToList();
            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();

            result.Orders = Mapper.Map<IEnumerable<Order>>(orders);
            CalculateRowNo(result, result.Orders, page, size, totalCount);

            return result;

        }

        public List<MemberModel> MemberOrderSearch(string mobileOrName, DateTime? dateFrom, DateTime? dateTo, int roleId)
        {
            List<MemberModel> list = new List<MemberModel>();
            MemberModel model = null;
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);

            List<MemberEntity> memberList = memberRepo.GetAll().ToList();
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                memberList = memberList.Where(m => m.Mobile == mobileOrName || m.MemberName.ToUpper().Contains(mobileOrName.ToUpper())).ToList();

                if (memberList == null)
                {
                    return list;
                }
            }

            if (roleId != -1)
                memberList = memberList.Where(m => m.RoleID == roleId).ToList();


            List<OrderEntity> orderList = orderRepo.GetAll().ToList();
            if (dateFrom.HasValue)
            {
                orderList = orderList.Where(o => o.CreateOn >= dateFrom.Value).ToList();
            }

            if (dateTo.HasValue)
            {
                orderList = orderList.Where(o => o.CreateOn <= dateTo.Value.AddDays(1)).ToList();
            }

            memberList.ForEach(m => m.TotalQuantity = orderList.Where(o => o.MemberID == m.ID && o.IsDeliverly == true).Sum(o => o.Quantity));
            memberList = memberList.Where(m => m.TotalQuantity > 0).ToList();

            foreach (var member in memberList ?? Enumerable.Empty<MemberEntity>())
            {
                if (member.RoleID <= 0) continue;
                model = new MemberModel();
                model.Mobile = member.Mobile;
                model.MemberName = member.MemberName;
                model.TotalQuantity = member.TotalQuantity;
                model.MemberRoleID = member.RoleID;
                model.IdentityNo = member.IdentityNo;
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取个人订单
        /// </summary>
        /// <param name="isDeliverly"></param>
        /// <returns></returns>
        public List<MemberOrderModel> GetPersonalOrder(bool? isDeliverly)
        {
            List<MemberOrderModel> list = new List<MemberOrderModel>();
            MemberOrderModel model = null;
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            var companyMember = memberRepo.FindBy(o => o.RoleID == 0).First();
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            var orderList = orderRepo.FindBy(o => o.MemberID == this.CurrentUser.MemberID).OrderByDescending(o => o.CreateOn);
            if (isDeliverly.HasValue)
            {
                orderList = orderList.Where(o => o.IsDeliverly == isDeliverly.Value).OrderByDescending(o => o.CreateOn);
            }

            MemberEntity sendMember = null;
            foreach (OrderEntity order in orderList ?? Enumerable.Empty<OrderEntity>())
            {
                sendMember = memberRepo.FindBy(m => m.ID == order.SendMemberID).FirstOrDefault();
                model = new MemberOrderModel();
                model.CreateOn = order.CreateOn.Value;
                model.ID = order.ID;
                model.IsSubmit = order.IsDeliverly;

                model.Quantity = order.Quantity;
                //model.SendMemberMobile = sendMember == null ? ConfigurationManager.AppSettings[GlobalConstants.CompanyTel] : sendMember.Mobile;
                //model.SendMemberName = sendMember == null ? ConfigurationManager.AppSettings[GlobalConstants.CompanyName] : sendMember.MemberName;
                ////model.Price = order.Price;
                model.SendMemberMobile = sendMember == null ? companyMember.Mobile : sendMember.Mobile;
                model.SendMemberName = sendMember == null ? companyMember.MemberName : sendMember.MemberName;

                model.Description = order.Description;
                model.Total = order.Total;
                model.Express = order.Express;
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// get personal orders
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="isDeliverly">if the order has been delivered</param>
        /// <returns></returns>
        public OrderSummary GetPersonalOrder1(int page, int rows, bool? isDeliverly)
        {
            var result = new OrderSummary();

            var orderDb = new Repository<OrderEntity>(Uow);

            Expression<Func<OrderEntity, bool>> whereExp = o => o.MemberID == this.CurrentUser.MemberID;
            if (isDeliverly.HasValue)
            {
                whereExp = whereExp.And(o => o.IsDeliverly == isDeliverly.Value);
            }

            var totalCount = orderDb.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

            var orders = orderDb.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * rows).Take(rows).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();

            result.Orders = Mapper.Map<IEnumerable<Order>>(orders);
            CalculateRowNo(result, result.Orders, page, rows, totalCount);

            result.AllDeliverly = true;
            //有未发货的订单
            if (orderDb.FindBy(o => o.MemberID == this.CurrentUser.MemberID && o.IsDeliverly == false).Any())
            {
                result.AllDeliverly = false;
            }
            return result;
        }

        /// <summary>
        /// 获取子代理订单
        /// </summary>
        /// <param name="createBy"></param>
        /// <param name="idCardNo"></param>
        /// <param name="hasChecked"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public List<MemberOrderModel> GetChildMemberOrder(string mobile, int iSubmit, ref int total, int page = 1, int rows = 10)
        {
            List<MemberOrderModel> list = new List<MemberOrderModel>();
            MemberOrderModel model = null;
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<ChildOrderEntity> orderRepo = new Repository<ChildOrderEntity>(Uow);
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                var member = memberRepo.FindBy(m => m.Mobile == mobile).FirstOrDefault();

                if (member == null)
                {
                    total = 0;
                    return list;
                }
                int iCount = 0;
                var orderList = orderRepo.FindBy(o => o.MemberID == member.ID).OrderByDescending(o => o.CreateOn).ToList();
                if (iSubmit != -1)
                {
                    bool bSubmit = false;
                    if (iSubmit == 1)
                    {
                        bSubmit = true;
                    }
                    orderList = orderList.Where(o => o.IsDeliverly == bSubmit).ToList();
                }
                foreach (ChildOrderEntity order in orderList ?? Enumerable.Empty<ChildOrderEntity>())
                {
                    model = new MemberOrderModel();
                    model.Address = member.Address;
                    model.CreateOn = order.CreateOn.Value;
                    model.ID = order.ID;
                    model.IsSubmit = order.IsDeliverly;
                    model.MemberName = member.MemberName;
                    model.Mobile = member.Mobile;
                    model.Quantity = order.Quantity;
                    model.MemberRoleID = member.RoleID;
                    list.Add(model);
                    iCount++;
                }
                total = iCount;
                return list;
            }
            else
            {
                var orderList = orderRepo.GetAll().OrderByDescending(o => o.CreateOn);
                if (iSubmit != -1)
                {
                    bool bSubmit = false;
                    if (iSubmit == 1)
                    {
                        bSubmit = true;
                    }
                    orderList = orderList.Where(o => o.IsDeliverly == bSubmit).OrderByDescending(o => o.CreateOn);
                }
                MemberEntity member = null;
                int iCount = 0;
                foreach (ChildOrderEntity order in orderList ?? Enumerable.Empty<ChildOrderEntity>())
                {
                    member = memberRepo.FindBy(m => m.ID == order.MemberID).FirstOrDefault();
                    model = new MemberOrderModel();
                    model.Address = member.Address;
                    model.CreateOn = order.CreateOn.Value;
                    model.ID = order.ID;
                    model.IsSubmit = order.IsDeliverly;
                    model.MemberName = member.MemberName;
                    model.Mobile = member.Mobile;
                    model.Quantity = order.Quantity;
                    model.MemberRoleID = member.RoleID;
                    list.Add(model);
                    iCount++;
                }
                total = iCount;
                return list;
            }
        }

        public bool CheckIDCard(string cardNo)
        {
            return IDCardUtil.CheckIDCard(cardNo);
        }

        /// <summary>
        /// 检查代理身份证号码是否有效
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns>yes有效no无效exception有异常</returns>
        public string IDCardValidate(string cardNo)
        {
            if (!IDCardUtil.CheckIDCard(cardNo))
            {
                return "无效的身份证号码";
            }
            return ExecuteResult.Success.ToString();
        }

        public ErrorCodes Update(MemberModel model)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            if (!IDCardUtil.CheckIDCard(model.IdentityCardNo))
            {
                return ErrorCodes.IDCardError;
            }
            var member = repo.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
            {
                return ErrorCodes.NotExist;
            }
            //member.BankCardNo = model.BankCardNo;
            //member.BankName = model.BankName;
            member.UpdateBy = model.UpdateBy;
            member.UpdateOn = DateTime.Now;
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            repo.Update(member);
            Uow.Commit();
            return ErrorCodes.Successed;
        }

        public ErrorCodes CheckedByOperator(Guid memberId)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);

            var member = repo.FindBy(p => p.ID == memberId).FirstOrDefault();
            if (member == null)
            {
                return ErrorCodes.NotExist;
            }
            repo.Update(member);
            Uow.Commit();
            return ErrorCodes.Successed;
        }

        /// <summary>
        /// 检查是否新代理
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public string IsNewMember(string cardNo)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);

            var member = repo.FindBy(m => m.Mobile == cardNo).FirstOrDefault();
            if (member == null) return "IsNewMember";
            else
            {
                var thisMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (member.CreateOn < thisMonthFirstDay) return "IsOldMember";
                else return "IsNewMember";
            }
        }

        public string UpdateMemberForCheck(MemberModel model)
        {
            throw new NotImplementedException();
        }

        public List<MemberModel> GetMemberForCheck(Guid createBy, string idCardNo, int hasChecked, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public List<MemberModel> GetMemberForHighNumber(string memberName, string idCardNo, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public string AddHeighNumber(string cardNo, string memberName, string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId, int agencyLevelId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加代理订单
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public string AddMemberOrder(string mobile, string quantity)
        {
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.Mobile == mobile).FirstOrDefault();
            if (member == null)
            {
                return "该手机号码不存在，请重新输入";
            }
            int iQuantity = 0;
            if (!int.TryParse(quantity, out iQuantity))
            {
                return "进货数量必须是数字，请重新输入";
            }
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
                {
                    List<MemberEntity> members = db.Database.SqlQuery<MemberEntity>("EXEC [P_GetFirstHigherAgentMember] @MemberID", new SqlParameter("@MemberID", member.ID)).ToList();
                    if (members != null && members.Any())
                    {
                        return string.Format("该用户必须向【{0},{1}】进货", members[0].Mobile, members[0].MemberName);
                    }
                }
            }
            if (iQuantity < memberRole.OneTimeAmount)
            {
                return string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount);
            }
            OrderEntity order = new OrderEntity();
            order.CreateBy = this.CurrentUser.ID;
            order.CreateOn = DateTime.Now;
            order.ID = Guid.NewGuid();
            order.IsDeliverly = false;
            order.MemberID = member.ID;
            order.Quantity = iQuantity;
            repo.Add(order);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="model"></param>
        public void AddPersonalOrder(AddPersonalOrderModel model)
        {
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);

            var currentUserMember = memberRepo.FindBy(m => m.ID == this.CurrentUser.MemberID).FirstOrDefault();
            Guid sendMemberId = Guid.Empty;
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == currentUserMember.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
                {
                    List<MemberEntity> members = db.Database.SqlQuery<MemberEntity>("EXEC [P_GetFirstHigherAgentMember] @MemberID", new SqlParameter("@MemberID", this.CurrentUser.MemberID)).ToList();
                    if (members != null && members.Any())
                    {
                        sendMemberId = members[0].ID;
                    }
                }

            }
            if (model.Quantity < memberRole.OneTimeAmount)
            {
                throw new Exception(string.Format("{0}单次进货数量不能小于{1}件", memberRole.RoleName, memberRole.OneTimeAmount));
            }
            OrderEntity order = new OrderEntity();
            order.CreateBy = this.CurrentUser.ID;
            order.CreateOn = DateTime.Now;
            order.ID = Guid.NewGuid();
            order.IsDeliverly = false;
            order.MemberID = this.CurrentUser.MemberID;
            order.SendMemberID = sendMemberId;
            order.Quantity = model.Quantity;
            int totalAmount = 0;
            order.Description = Common.GetTotalAmountDescription(memberRole.Price, memberRole.RoleRiseDescription, model.Quantity, currentUserMember.RoleID, (int)currentUserMember.CurrentRoleQuantity, out totalAmount);
            order.Total = totalAmount;
            repo.Add(order);
            Uow.Commit();
        }

        /// <summary>
        /// 添加子代理订单
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public string AddChildMemberOrder(string mobile, string quantity)
        {
            Repository<ChildOrderEntity> repo = new Repository<ChildOrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.Mobile == mobile).FirstOrDefault();
            if (member == null)
            {
                return "该手机号码不存在，请重新输入";
            }
            int iQuantity = 0;
            if (!int.TryParse(quantity, out iQuantity))
            {
                return "进货数量必须是数字，请重新输入";
            }
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                if (member.ParentMemberID.HasValue && member.ParentMemberID.Value != Guid.Empty)
                {
                    return "该用户级别不够，并且有上级用户，所以不允许直接向公司进货，请向上级进货";
                }
            }
            if (iQuantity < memberRole.OneTimeAmount)
            {
                return string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount);
            }
            ChildOrderEntity order = new ChildOrderEntity();
            order.CreateBy = this.CurrentUser.ID;
            order.CreateOn = DateTime.Now;
            order.ID = Guid.NewGuid();
            order.IsDeliverly = false;
            order.MemberID = member.ID;
            order.Quantity = iQuantity;
            repo.Add(order);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 发货时更新董事
        /// </summary>
        /// <param name="member"></param>
        /// <param name="newRoleId"></param>
        private void UpdateDirectorForSend(MemberEntity member, int newRoleId)
        {
            bool bUpdate = false;
            //1.之前小于省代，现在大于等于省代，更新
            if (member.RoleID < (int)enmMemberRole.Province)
            {
                if (newRoleId >= (int)enmMemberRole.Province)
                {
                    bUpdate = true;
                }
            }
            //2.之前等于省代，现在大于省代，更新
            else if (member.RoleID == (int)enmMemberRole.Province)
            {
                if (newRoleId > (int)enmMemberRole.Province)
                {
                    bUpdate = true;
                }
            }
            if (bUpdate)
            {
                MemberEntity _member = new MemberEntity();
                _member.Address = member.Address;
                _member.ChildTotalQuantity = member.ChildTotalQuantity;
                _member.CreateBy = member.CreateBy;
                _member.CreateOn = member.CreateOn;
                _member.CurrentRoleQuantity = member.CurrentRoleQuantity;
                _member.GeneralAvailable = member.GeneralAvailable;
                _member.ID = member.ID;
                _member.IdentityNo = member.IdentityNo;
                _member.MemberName = member.MemberName;
                _member.Mobile = member.Mobile;
                _member.ParentMemberID = member.ParentMemberID;
                _member.PreviousRoleID = member.PreviousRoleID;
                _member.PreviousRoleQuantity = member.PreviousRoleQuantity;
                _member.ProvinceAvailable = member.ProvinceAvailable;
                _member.RoleID = newRoleId;
                _member.TotalCount = member.TotalCount;
                _member.TotalQuantity = member.TotalQuantity;
                _member.UpdateBy = member.UpdateBy;
                _member.UpdateOn = member.UpdateOn;
                _member.ValidRole = member.ValidRole;
                UpdateDirector(new List<MemberEntity>() { _member });
            }
        }

        /// <summary>
        /// 发货时更新市代及以上数量
        /// </summary>
        /// <param name="member"></param>
        /// <param name="newRoleId"></param>
        private void UpdateCityAgentCountForSend(MemberEntity member, int newRoleId)
        {
            bool bUpdate = false;
            //1.之前小于市代，现在大于等于市代，更新
            if (member.RoleID < (int)enmMemberRole.City)
            {
                if (newRoleId >= (int)enmMemberRole.City)
                {
                    bUpdate = true;
                }
            }

            if (bUpdate)
            {
                MemberEntity _member = new MemberEntity();
                _member.Address = member.Address;
                _member.ChildTotalQuantity = member.ChildTotalQuantity;
                _member.CreateBy = member.CreateBy;
                _member.CreateOn = member.CreateOn;
                _member.CurrentRoleQuantity = member.CurrentRoleQuantity;
                _member.GeneralAvailable = member.GeneralAvailable;
                _member.ID = member.ID;
                _member.IdentityNo = member.IdentityNo;
                _member.MemberName = member.MemberName;
                _member.Mobile = member.Mobile;
                _member.ParentMemberID = member.ParentMemberID;
                _member.PreviousRoleID = member.PreviousRoleID;
                _member.PreviousRoleQuantity = member.PreviousRoleQuantity;
                _member.ProvinceAvailable = member.ProvinceAvailable;
                _member.RoleID = newRoleId;
                _member.TotalCount = member.TotalCount;
                _member.TotalQuantity = member.TotalQuantity;
                _member.UpdateBy = member.UpdateBy;
                _member.UpdateOn = member.UpdateOn;
                _member.ValidRole = member.ValidRole;
                UpdateCityAgentCount(new List<MemberEntity>() { _member });
            }
        }

        /// <summary>
        /// 获取上面第一个董事(上个月及上个月之前升董事)
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private Guid GetFirstDirector(Guid memberId)
        {
            Guid result = Guid.Empty;
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                result = db.Database.SqlQuery<Guid>("EXEC [P_GetFirstDirector] @MemberId", new SqlParameter("@MemberId", memberId)).FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// 获取上面三个总代(及以上)
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private List<Guid> GetThreeGeneralAgent(Guid memberId)
        {
            List<Guid> result = null;
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                result = db.Database.SqlQuery<Guid>("EXEC [P_GetThreeGeneralAgent] @MemberId", new SqlParameter("@MemberId", memberId)).ToList();
            }
            return result;
        }

        /// <summary>
        /// 设置上面3个获奖的总代(50,30,20)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="memberId"></param>
        private void UpdateAwardParents(Guid orderId, Guid memberId)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            var parentIds = GetThreeGeneralAgent(memberId);
            if (parentIds != null && parentIds.Any())
            {
                var order = orderRepo.FindBy(o => o.ID == orderId).First();
                int index = 1;
                foreach (var parentId in parentIds)
                {
                    switch (index)
                    {
                        case 1:
                            {
                                order.GeneralAgent1ID = parentId;
                                break;
                            }
                        case 2:
                            {
                                order.GeneralAgent2ID = parentId;
                                break;
                            }
                        case 3:
                            {
                                order.GeneralAgent3ID = parentId;
                                break;
                            }
                        default:
                            break;

                    }
                    index++;
                }
                var directorId = GetFirstDirector(memberId);
                if (directorId != Guid.Empty)
                {
                    order.DirectorID = directorId;
                }
                orderRepo.Update(order);
                Uow.Commit();
            }
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="quantity"></param>
        /// <param name="isSubmit"></param>
        /// <returns></returns>
        public void SendMemberOrder(Guid orderId, string expressContent)
        {
            if (string.IsNullOrWhiteSpace(expressContent))
            {
                throw new Exception("快递信息不能为空");
            }

            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            Repository<RoleUpgradeEntity> ruRepo = new Repository<RoleUpgradeEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                throw new Exception("该订单不存在，请联系管理员");
            }
            if (order.IsDeliverly)
            {
                throw new Exception("该订单已经发货，不允许再次发货");
            }
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            var memberRoleId = member.RoleID;
            var sendMemberId = order.SendMemberID;
            var generalAvailable = member.GeneralAvailable;
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }

            if (order.SendMemberID != Guid.Empty)
            {
                if (order.SendMemberID != this.CurrentUser.MemberID)
                {
                    throw new Exception("您不是该订单的发货代理，不能向该订单发货");
                }
                var currentUserMember = memberRepo.FindBy(p => p.ID == this.CurrentUser.MemberID).FirstOrDefault();
                if (currentUserMember.TotalQuantity < order.Quantity)
                {
                    throw new Exception(string.Format("您库存只有{0}件，少于{1}件，不能发货", currentUserMember.TotalQuantity, order.Quantity));
                }
                currentUserMember.TotalQuantity -= order.Quantity;
                memberRepo.Update(currentUserMember);
            }

            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();


            order.IsDeliverly = true;
            order.Express = expressContent;
            order.SendDate = DateTime.Now;
            repo.Update(order);
            if (!string.IsNullOrWhiteSpace(memberRole.RoleRiseDescription))
            {

                //member.PreviousRoleQuantity = member.CurrentRoleQuantity;
                decimal iCurrentQuantity = 0;
                int iRoleID = Common.GetRoleID(memberRole.RoleRiseDescription, order.Quantity + member.CurrentRoleQuantity, member.RoleID, out iCurrentQuantity);
                if (iRoleID != 0)
                {
                    if (member.RoleID != iRoleID)
                    {
                        //更新董事
                        UpdateDirectorForSend(member, iRoleID);

                        //member.PreviousRoleID = member.RoleID;
                        RoleUpgradeEntity ruEntity = new RoleUpgradeEntity();
                        ruEntity.CreateBy = this.CurrentUser.ID;
                        ruEntity.CreateOn = DateTime.Now;
                        ruEntity.CurrentRoleId = iRoleID;
                        ruEntity.OriginalRoleId = member.RoleID;
                        ruEntity.MemberId = member.ID;
                        ruRepo.Add(ruEntity);
                        member.RoleID = iRoleID;
                    }
                    member.CurrentRoleQuantity = iCurrentQuantity;
                }
                else
                {
                    member.CurrentRoleQuantity += order.Quantity;
                }
            }

            member.TotalQuantity += order.Quantity;
            member.TotalCount += order.Quantity;
            if (member.TotalCount >= GlobalConstants.ProvinceCount && member.TotalCount < GlobalConstants.GeneralCount)
            {
                member.ProvinceAvailable = 1;
            }
            else if (member.TotalCount >= GlobalConstants.GeneralCount)
            {
                member.ProvinceAvailable = 1;
                member.GeneralAvailable = 1;
            }
            memberRepo.Update(member);

            Uow.Commit();

            if (memberRoleId >= (int)enmMemberRole.GeneralAgent && generalAvailable > 0 && sendMemberId == Guid.Empty)
            {
                UpdateAwardParents(order.ID, member.ID);
            }
        }

        /// <summary>
        /// 拷贝一份代理
        /// </summary>
        /// <param name="oriMember">旧的会员实体</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        private MemberEntity CopyMember(MemberEntity oriMember, int roleId)
        {
            var result = new MemberEntity();
            result.Address = oriMember.Address;
            result.ChildTotalQuantity = oriMember.ChildTotalQuantity;
            result.CreateBy = oriMember.CreateBy;
            result.CreateOn = oriMember.CreateOn;
            result.CurrentRoleQuantity = oriMember.CurrentRoleQuantity;
            result.GeneralAvailable = oriMember.GeneralAvailable;
            result.ID = oriMember.ID;
            result.IdentityNo = oriMember.IdentityNo;
            result.MemberName = oriMember.MemberName;
            result.Mobile = oriMember.Mobile;
            result.ParentMemberID = oriMember.ParentMemberID;
            result.PreviousRoleID = oriMember.PreviousRoleID;
            result.PreviousRoleQuantity = oriMember.PreviousRoleQuantity;
            result.ProvinceAvailable = oriMember.ProvinceAvailable;
            result.RoleID = roleId;
            result.TotalCount = oriMember.TotalCount;
            result.TotalQuantity = oriMember.TotalQuantity;
            result.UpdateBy = oriMember.UpdateBy;
            result.UpdateOn = oriMember.UpdateOn;
            result.ValidRole = oriMember.ValidRole;
            return result;
        }



        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="quantity"></param>
        /// <param name="isSubmit"></param>
        /// <returns></returns>
        public void SendMemberOrder1(Guid orderId, string expressContent)
        {
            if (string.IsNullOrWhiteSpace(expressContent))
            {
                throw new Exception("快递信息不能为空");
            }

            Repository<OrderEntity> orderDb = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberDb = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> roleDb = new Repository<MemberRoleEntity>(Uow);
            Repository<RoleUpgradeEntity> ruDb = new Repository<RoleUpgradeEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                throw new Exception("该订单不存在，请联系管理员");
            }
            if (order.IsDeliverly)
            {
                throw new Exception("该订单已经发货，不允许再次发货");
            }
            //判断手机号是不是存在
            var member = memberDb.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            var memberRoleId = member.RoleID;
            var sendMemberId = order.SendMemberID;
            var generalAvailable = member.GeneralAvailable;
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }

            var odDb = new Repository<OrderDetailsEntity>(Uow);

            //检查库存是否够用
            var mpDb = new Repository<MemberProductEntity>(Uow);
            MemberProductEntity sendMp = null;
            MemberProductEntity buyMp = null;

            var odList = odDb.FindBy(o => o.OrderID == orderId).ToList();
            int quantity = 0;

            if (order.SendMemberID != Guid.Empty)
            {
                if (order.SendMemberID != this.CurrentUser.MemberID)
                {
                    throw new Exception("您不是该订单的发货代理，不能向该订单发货");
                }



                // 更新库存
                foreach (var od in odList)
                {
                    var mpList = mpDb.FindBy(o => o.MemberID == this.CurrentUser.MemberID).ToList();
                    if (mpList.Any(o => o.ProductID == od.ProductID && o.Quantity >= od.Quantity))
                    {
                        // 发货代理减库存
                        sendMp = mpList.First(o => o.ProductID == od.ProductID);
                        sendMp.Quantity -= od.Quantity;
                        mpDb.Update(sendMp);

                        // 购买代理加库存
                        if (mpDb.FindBy(o => o.MemberID == member.ID && o.ProductID == od.ProductID).Any())
                        {
                            buyMp = mpDb.FindBy(o => o.MemberID == member.ID && o.ProductID == od.ProductID).First();
                            buyMp.Quantity += od.Quantity;
                            mpDb.Update(buyMp);
                        }
                        else
                        {
                            buyMp = new MemberProductEntity();
                            buyMp.CreateBy = this.CurrentUser.ID;
                            buyMp.CreateOn = DateTime.Now;
                            buyMp.ID = Guid.NewGuid();
                            buyMp.MemberID = member.ID;
                            buyMp.ProductID = od.ProductID;
                            buyMp.Quantity = od.Quantity;
                            mpDb.Add(buyMp);
                        }

                        continue;
                    }
                    if (mpList.Any(o => o.ProductID == od.ProductID))
                    {
                        quantity = mpList.First(o => o.ProductID == od.ProductID).Quantity;
                    }
                    throw new Exception(string.Format("【{0}】库存只有{1}件，少于{2}件，不能发货", od.Product.Name, quantity, od.Quantity));
                }

            }
            else
            {
                // 更新库存
                foreach (var od in odList)
                {

                    // 加库存
                    if (mpDb.FindBy(o => o.MemberID == member.ID && o.ProductID == od.ProductID).Any())
                    {
                        buyMp = mpDb.FindBy(o => o.MemberID == member.ID && o.ProductID == od.ProductID).First();
                        buyMp.Quantity += od.Quantity;
                        mpDb.Update(buyMp);
                    }
                    else
                    {
                        buyMp = new MemberProductEntity();
                        buyMp.CreateBy = this.CurrentUser.ID;
                        buyMp.CreateOn = DateTime.Now;
                        buyMp.ID = Guid.NewGuid();
                        buyMp.MemberID = member.ID;
                        buyMp.ProductID = od.ProductID;
                        buyMp.Quantity = od.Quantity;
                        mpDb.Add(buyMp);
                    }
                }
            }

            var memberRole = roleDb.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();

            // 更新订单
            order.IsDeliverly = true;
            order.Express = expressContent;
            order.SendDate = DateTime.Now;
            orderDb.Update(order);

            // 订单总额
            var orderAmount = odDb.FindBy(o => o.OrderID == orderId).Sum(o => o.Quantity * o.Product.Price);

            // 单位金额
            var scDb = new Repository<SystemConfigEntity>(Uow);
            var unitAmount = decimal.Parse(scDb.FindBy(o => o.Name == SystemSettingConstants.Price).First().ConfigValue);

            // 省代数量
            var provinceCount = roleDb.FindBy(o => o.ID < (int)enmMemberRole.Province).Sum(o => o.UpgradeCount);

            // 总代数量
            var generalCount = roleDb.FindBy(o => o.ID < (int)enmMemberRole.GeneralAgent).Sum(o => o.UpgradeCount);

            if (!string.IsNullOrWhiteSpace(memberRole.RoleRiseDescription))
            {
                decimal roleAmount = 0;

                // 检查是否升级

                MemberEntity newMember = null;

                // 市代数量
                var cityCount = roleDb.FindBy(o => o.ID < (int)enmMemberRole.City).Sum(o => o.UpgradeCount);


                // 总金额
                var totalAmount = member.TotalAmount;

                // 市代金额
                var cityAmount = unitAmount * cityCount;

                // 总金额 < 市代金额
                if (totalAmount < cityAmount)
                {
                    // 总金额 + 订单金额 >= 市代金额
                    if (totalAmount + orderAmount >= cityAmount)
                    {
                        newMember = CopyMember(member, (int)enmMemberRole.City);

                        //更新市代数量
                        UpdateCityAgentCount(new List<MemberEntity>() { newMember });
                    }
                }

                newMember = null;

                // 省代金额
                var provinceAmount = unitAmount * (provinceCount - member.CityMinus);

                // 总代金额
                var generalAmount = unitAmount * (generalCount - member.CityMinus);

                // 优惠金额
                var minusAmount = member.CityMinus * unitAmount;

                // 之前总金额小于省代，进货之后总金额大于省代但小于总代
                if ((member.TotalAmount < provinceAmount) &&
                member.TotalAmount + orderAmount >= provinceAmount &&
                member.TotalAmount + orderAmount < generalAmount)
                {
                    // 以省代级别更新董事
                    newMember = CopyMember(member, GlobalConstants.ProvinceRoleID);
                }

                // 之前总金额小于省代，进货之后总金额大于总代
                else if ((member.TotalAmount < provinceAmount) &&
                member.TotalAmount + orderAmount >= generalAmount)
                {
                    // 以总代级别更新董事
                    newMember = CopyMember(member, GlobalConstants.GeneralRoleID);
                }

                // 之前总金额大于省代但小于总代，进货之后总金额大于总代
                else if ((member.TotalAmount >= provinceAmount) &&
                    (member.TotalAmount < generalAmount) &&
                member.TotalAmount + orderAmount >= generalAmount)
                {
                    // 升级不需要优惠金额
                    minusAmount = 0;

                    // 以总代级别更新董事
                    newMember = CopyMember(member, GlobalConstants.GeneralRoleID);
                }
                else
                {
                    // 升级不能用优惠金额
                    minusAmount = 0;
                }

                if (newMember != null)
                {
                    UpdateDirector(new List<MemberEntity>() { newMember });
                }

                int iRoleID = (int)enmMemberRole.Customer;

                iRoleID = Common.GetRoleID1(memberRole.RoleRiseDescription, orderAmount + member.CurrentRoleAmount, unitAmount, minusAmount, member.RoleID, out roleAmount);

                if (member.RoleID != iRoleID)
                {
                    RoleUpgradeEntity ruEntity = new RoleUpgradeEntity();
                    ruEntity.CreateBy = this.CurrentUser.ID;
                    ruEntity.CreateOn = DateTime.Now;
                    ruEntity.CurrentRoleId = iRoleID;
                    ruEntity.OriginalRoleId = member.RoleID;
                    ruEntity.MemberId = member.ID;
                    ruDb.Add(ruEntity);
                }

                member.RoleID = iRoleID;
                member.CurrentRoleAmount = roleAmount;

            }

            // 更新总金额
            member.TotalAmount += orderAmount;

            // 更新有效总代和有效省代
            if (member.TotalAmount >= (provinceCount - member.CityMinus) * unitAmount && member.TotalAmount < (generalCount - member.CityMinus) * unitAmount)
            {
                member.ProvinceAvailable = 1;
            }
            else if (member.TotalAmount >= (generalCount - member.CityMinus) * unitAmount)
            {
                member.ProvinceAvailable = 1;
                member.GeneralAvailable = 1;
            }
            memberDb.Update(member);

            // 更新识别码
            using (SimpleWebUnitOfWork database = new SimpleWebUnitOfWork())
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@orderId", orderId));
                database.Database.ExecuteSqlCommand("EXEC [P_UpdateCodes] @orderId", parameters.ToArray());
            }

            Uow.Commit();

            // 更新奖金
            if (memberRoleId >= (int)enmMemberRole.GeneralAgent && generalAvailable > 0 && sendMemberId == Guid.Empty)
            {
                UpdateAwardParents(order.ID, member.ID);
            }
        }

        /// <summary>
        /// 确认收款
        /// </summary>
        /// <param name="orderId"></param>
        public void FinancialConfirm(Guid orderId)
        {
            Repository<OrderEntity> orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                throw new Exception("该订单不存在，请联系管理员");
            }
            if (order.FinancialStatus == (int)OrderFinancialStatus.Paid)
            {
                throw new Exception("该订单已经确认收款");
            }
            if (order.IsDeliverly)
            {
                throw new Exception("该订单已经发货，不允许确认收款");
            }

            order.FinancialStatus = (int)OrderFinancialStatus.Paid;
            orderDb.Update(order);

            Uow.Commit();


        }

        public void UpdateOrderExpress(Guid orderId, string expressContent)
        {
            if (string.IsNullOrWhiteSpace(expressContent))
            {
                throw new Exception("快递信息不能为空");
            }
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);

            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            order.UpdateBy = this.CurrentUser.ID;
            order.UpdateOn = DateTime.Now;
            order.Express = expressContent;
            repo.Update(order);
            Uow.Commit();
        }

        /// <summary>
        /// 子代理发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="quantity"></param>
        /// <param name="isSubmit"></param>
        /// <returns></returns>
        public string SendChildMemberOrder(Guid orderId)
        {
            Repository<ChildOrderEntity> repo = new Repository<ChildOrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                return "该订单不存在，请联系管理员";
            }
            if (order.IsDeliverly)
            {
                return "该订单已经发货，不允许再次发货";
            }
            var currentUserMember = memberRepo.FindBy(p => p.ID == this.CurrentUser.MemberID).FirstOrDefault();
            if (currentUserMember == null)
            {
                return "您自己不是代理，不能给别人发货";
            }
            if (currentUserMember.TotalQuantity < order.Quantity)
            {
                return string.Format("您库存只有{0}件，少于{1}件，不能发货", currentUserMember.TotalQuantity, order.Quantity);
            }
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            if (member == null)
            {
                return "该代理不存在，请联系管理员";
            }

            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();
            //if (!memberRole.AllowedDirectOrder)
            //{
            //    if (member.ParentMemberID.HasValue)
            //    {
            //        return "该用户不允许直接向公司进货，请向上级进货";
            //    }
            //}
            if (order.Quantity < memberRole.OneTimeAmount)
            {
                return string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount);
            }
            currentUserMember.TotalQuantity -= order.Quantity;
            memberRepo.Update(currentUserMember);

            order.IsDeliverly = true;
            order.SendMemberID = this.CurrentUser.MemberID;
            repo.Update(order);
            if (!string.IsNullOrWhiteSpace(memberRole.RoleRiseDescription))
            {

                //member.PreviousRoleQuantity = member.CurrentRoleQuantity;
                decimal iCurrentQuantity = 0;
                int iRoleID = Common.GetRoleID(memberRole.RoleRiseDescription, order.Quantity + (int)member.CurrentRoleQuantity, member.RoleID, out iCurrentQuantity);
                if (iRoleID != 0)
                {
                    if (member.RoleID != iRoleID)
                    {
                        //member.PreviousRoleID = member.RoleID;
                        member.RoleID = iRoleID;
                    }
                    member.CurrentRoleQuantity = iCurrentQuantity;
                }
                else
                {
                    member.CurrentRoleQuantity += order.Quantity;
                }
                member.TotalQuantity += order.Quantity;
                memberRepo.Update(member);
            }

            Uow.Commit();
            return ExecuteResult.Success.ToString();

        }

        /// <summary>
        /// 修改代理订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public string UpdateMemberOrder(Guid orderId, string quantity)
        {
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                return "该订单不存在，请联系管理员";
            }
            if (order.IsDeliverly)
            {
                return "该订单已经发货，不允许修改";
            }
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            if (member == null)
            {
                return "该代理不存在，请联系管理员";
            }
            int iQuantity = 0;
            if (!int.TryParse(quantity, out iQuantity))
            {
                return "进货数量必须是数字，请重新输入";
            }
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                if (member.ParentMemberID.HasValue && member.ParentMemberID.Value != Guid.Empty)
                {
                    return "该用户级别不够，并且有上级用户，所以不允许直接向公司进货，请向上级进货";
                }
            }
            if (iQuantity < memberRole.OneTimeAmount)
            {
                return string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount);
            }

            order.UpdateBy = this.CurrentUser.ID;
            order.UpdateOn = DateTime.Now;
            order.Quantity = iQuantity;
            repo.Update(order);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 修改子代理订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public string UpdateChildMemberOrder(Guid orderId, string quantity)
        {
            Repository<ChildOrderEntity> repo = new Repository<ChildOrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                return "该订单不存在，请联系管理员";
            }
            if (order.IsDeliverly)
            {
                return "该订单已经发货，不允许修改";
            }
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            if (member == null)
            {
                return "该代理不存在，请联系管理员";
            }
            int iQuantity = 0;
            if (!int.TryParse(quantity, out iQuantity))
            {
                return "进货数量必须是数字，请重新输入";
            }
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == member.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                if (member.ParentMemberID.HasValue && member.ParentMemberID.Value != Guid.Empty)
                {
                    return "该用户级别不够，并且有上级用户，所以不允许直接向公司进货，请向上级进货";
                }
            }
            if (iQuantity < memberRole.OneTimeAmount)
            {
                return string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount);
            }

            order.UpdateBy = this.CurrentUser.ID;
            order.UpdateOn = DateTime.Now;
            order.Quantity = iQuantity;
            repo.Update(order);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 删除代理订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void RemoveMemberOrder(Guid orderId)
        {
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                throw new Exception("该订单不存在，请联系管理员");
            }
            if (order.IsDeliverly)
            {
                throw new Exception("该订单已经发货，不允许删除");
            }
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }
            repo.Remove(order);
            Uow.Commit();
        }

        /// <summary>
        /// 删除子代理订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string RemoveChildMemberOrder(Guid orderId)
        {
            Repository<ChildOrderEntity> repo = new Repository<ChildOrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);
            var order = repo.FindBy(o => o.ID == orderId).FirstOrDefault();
            if (order == null)
            {
                return "该订单不存在，请联系管理员";
            }
            if (order.IsDeliverly)
            {
                return "该订单已经发货，不允许删除";
            }
            //判断手机号是不是存在
            var member = memberRepo.FindBy(p => p.ID == order.MemberID).FirstOrDefault();
            if (member == null)
            {
                return "该代理不存在，请联系管理员";
            }
            repo.Remove(order);
            member.CurrentRoleQuantity = member.PreviousRoleQuantity;
            member.RoleID = member.PreviousRoleID;
            member.TotalQuantity -= order.Quantity;
            memberRepo.Update(member);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }


        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddMember(Member model)
        {

            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("姓名必须输入");
            }
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            MemberEntity parent = null;
            if (!string.IsNullOrWhiteSpace(model.ParentMember.Mobile))
            {
                long iParentMobile = 0;
                if (!long.TryParse(model.ParentMember.Mobile, out iParentMobile))
                {
                    throw new Exception("上级手机号码必须是数字");
                }
                if (model.ParentMember.Mobile.Length != 11)
                {
                    throw new Exception("上级手机号码长度必须是11位");
                }

                parent = repo.FindBy(p => p.Mobile == model.ParentMember.Mobile).FirstOrDefault();
                if (parent == null)
                {
                    throw new Exception("上级手机号码输入有误，请重新输入");
                }
                if (model.Mobile == model.ParentMember.Mobile)
                {
                    throw new Exception("上级手机号码不能和本人手机号码一样");
                }
            }

            #region Check if IdentityNo has beeen used
            if (!string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (repo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            var member = new MemberEntity();
            member.ID = Guid.NewGuid();
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = DateTime.Now;
            if (parent != null)
            {
                member.ParentMemberID = parent.ID;
            }
            member.MemberName = model.MemberName;
            member.RoleID = model.RoleID;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            member.ProvinceAvailable = model.ProvinceAvailable;
            member.GeneralAvailable = model.GeneralAvailable;
            member.TotalQuantity = model.TotalQuantity;
            member.CurrentRoleQuantity = model.CurrentRoleQuantity;

            repo.Add(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            if (userRepo.FindBy(u => u.Account == member.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("该手机号码已经存在，请重新输入");
            }
            UserEntity user = new UserEntity();
            user.Account = member.Mobile;
            user.CreateBy = this.CurrentUser.ID;
            user.CreateOn = DateTime.Now;
            user.ID = Guid.NewGuid();
            user.MemberID = member.ID;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            userRepo.Add(user);
            Uow.Commit();
        }

        /// <summary>
        /// 添加代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void AddMember1(Member model)
        {

            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("姓名必须输入");
            }
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            MemberEntity parent = null;
            if (!string.IsNullOrWhiteSpace(model.ParentMember.Mobile))
            {
                long iParentMobile = 0;
                if (!long.TryParse(model.ParentMember.Mobile, out iParentMobile))
                {
                    throw new Exception("上级手机号码必须是数字");
                }
                if (model.ParentMember.Mobile.Length != 11)
                {
                    throw new Exception("上级手机号码长度必须是11位");
                }

                parent = repo.FindBy(p => p.Mobile == model.ParentMember.Mobile).FirstOrDefault();
                if (parent == null)
                {
                    throw new Exception("上级手机号码输入有误，请重新输入");
                }
                if (model.Mobile == model.ParentMember.Mobile)
                {
                    throw new Exception("上级手机号码不能和本人手机号码一样");
                }
            }

            #region Check if IdentityNo has beeen used
            if (!string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (repo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            var member = new MemberEntity();
            member.ID = Guid.NewGuid();
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = DateTime.Now;
            if (parent != null)
            {
                member.ParentMemberID = parent.ID;
            }
            member.MemberName = model.MemberName;
            member.RoleID = model.RoleID;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            member.ProvinceAvailable = model.ProvinceAvailable;
            member.GeneralAvailable = model.GeneralAvailable;
            member.CurrentRoleAmount = model.CurrentRoleAmount;
            member.TotalAmount = model.TotalAmount; // 总金额

            repo.Add(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            if (userRepo.FindBy(u => u.Account == member.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("该手机号码已经存在，请重新输入");
            }
            UserEntity user = new UserEntity();
            user.Account = member.Mobile;
            user.CreateBy = this.CurrentUser.ID;
            user.CreateOn = DateTime.Now;
            user.ID = Guid.NewGuid();
            user.MemberID = member.ID;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            userRepo.Add(user);
            Uow.Commit();
        }

        /// <summary>
        /// 按范围添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        public void AddCodeRange(IdentityCode model)
        {
            if (model.CodeFrom == 0)
            {
                throw new Exception("起始唯一识别码不能为空");
            }
            if (model.CodeTo == 0)
            {
                throw new Exception("结束唯一识别码不能为空");
            }
            if (model.CodeTo < model.CodeFrom)
            {
                throw new Exception("结束唯一识别码不能小于起始唯一识别码");
            }
            var db = new Repository<IdentityCodeEntity>(Uow);

            //需要插入的唯一识别码
            var codes = new List<long>();
            for (long i = model.CodeFrom; i <= model.CodeTo; i++)
            {
                codes.Add(i);
            }

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var orderDetails = odDb.FindBy(o => o.ID == model.OrderDetailsID).First();

            var currentCount = db.FindBy(o => o.OrderDetailsID == model.OrderDetailsID).Count();
            if (currentCount >= orderDetails.Quantity)
            {
                throw new Exception("该订单唯一识别码已经添加完毕，不能再添加");
            }

            if (orderDetails.Quantity < codes.Count() + currentCount)
            {
                throw new Exception("唯一识别码数量超过订货数量，请重新输入");
            }

            //判断当前用户是否有权限提供该唯一识别码
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {

                //允许使用的唯一识别码
                var allowedCodes = db.FindBy(o => o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID && o.Status == 0).Select(o => o.Code).ToList();

                //不允许使用的唯一识别码
                var unallowedCodes = codes.Where(o => !allowedCodes.Contains(o)).ToList();

                if (unallowedCodes.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    unallowedCodes.Each(o =>
                    {
                        sb.Append(o + ",");
                    });
                    throw new Exception("唯一识别码" + sb.ToString() + "不可用，请重新添加");
                }
            }
            else
            {
                //不允许使用唯一识别码
                var unallowedCodes = db.FindBy(o => codes.Contains(o.Code)).Select(o => o.Code).ToList();
                if (unallowedCodes.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    unallowedCodes.Each(o =>
                    {
                        sb.Append(o + ",");
                    });
                    throw new Exception("唯一识别码" + sb.ToString() + "已经存在，请重新添加");
                }
            }

            bool codeFull = false;
            //订单唯一识别码已经添加完成
            if (currentCount == orderDetails.Quantity - codes.Count())
            {
                codeFull = true;
            }

            //插入数据
            using (SimpleWebUnitOfWork database = new SimpleWebUnitOfWork())
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@codeFrom", model.CodeFrom));
                parameters.Add(new SqlParameter("@codeTo", model.CodeTo));
                parameters.Add(new SqlParameter("@createBy", this.CurrentUser.ID));
                parameters.Add(new SqlParameter("@orderDetailsId", model.OrderDetailsID));
                parameters.Add(new SqlParameter("@codeFull", codeFull));
                database.Database.ExecuteSqlCommand("EXEC [P_BulkAddCodes] @codeFrom, @codeTo, @createBy, @orderDetailsId, @codeFull", parameters.ToArray());
            }

            Uow.Commit();
        }

        /// <summary>
        /// 单个添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        public void AddCode(IdentityCode model)
        {
            if (model.Code == 0)
            {
                throw new Exception("唯一识别码不能为空");
            }
            var db = new Repository<IdentityCodeEntity>(Uow);

            var entity = new IdentityCodeEntity();

            var currentId = this.CurrentUser.ID;
            var now = DateTime.Now;

            //判断当前用户是否有权限提供该唯一识别码
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                if (!db.FindBy(o => model.Code == o.Code && o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID && o.Status == 0).Any())
                {
                    throw new Exception("唯一识别码" + model.Code + "不是有效的唯一识别码，请重新添加");
                }
                //当前用户不能再用这个唯一识别码
                var currEntity = db.FindBy(o => o.Code == model.Code && o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID).First();
                currEntity.Status = 1;
                db.Update(currEntity);
            }
            else
            {
                if (db.FindBy(o => o.Code == model.Code).Any())
                {
                    throw new Exception("唯一识别码" + model.Code + "已被使用，不能添加");
                }
            }

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var odEntity = odDb.FindBy(o => o.ID == model.OrderDetailsID).First();

            var currentCount = db.FindBy(o => o.OrderDetailsID == model.OrderDetailsID).Count();
            if (currentCount >= odEntity.Quantity)
            {
                throw new Exception("该产品唯一识别码已经添加完毕，不能再添加");
            }



            entity.CreateBy = currentId;
            entity.CreateOn = now;
            entity.Code = model.Code;
            entity.ID = Guid.NewGuid();
            entity.Status = 0;

            db.Add(entity);

            //订单明细唯一识别码已经添加完成
            if (currentCount == odEntity.Quantity - 1)
            {
                odEntity.Status = (int)OrderDetailsStatus.CodeFull;
                odEntity.UpdateBy = currentId;
                odEntity.UpdateOn = now;
                odDb.Update(odEntity);

                //订单唯一识别码已经添加完成
                var count = odDb.FindBy(o => o.OrderID == odEntity.OrderID && o.Status == (int)OrderDetailsStatus.CodeNotFull).Count();
                if (count <= 1)
                {
                    var orderDb = new Repository<OrderEntity>(Uow);
                    var order = orderDb.FindBy(o => o.ID == odEntity.OrderID).First();
                    order.Status = (int)OrderDetailsStatus.CodeFull;
                    order.UpdateBy = currentId;
                    order.UpdateOn = now;
                    orderDb.Update(order);
                }

            }



            Uow.Commit();

        }

        /// <summary>
        /// 添加子代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddChildMember(string mobile, string memberName, string parentMobile, string address)
        {

            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            //判断手机号是不是存在
            var idCardNo = repo.FindBy(p => p.Mobile == mobile).FirstOrDefault();
            if (idCardNo != null)
            {
                return "该手机号已经存在，请重新输入";
            }
            List<MemberEntity> members = null;
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                members = db.Database.SqlQuery<MemberEntity>("EXEC [P_GetChildMembers] @MemberID", new SqlParameter("@MemberID", this.CurrentUser.ID)).ToList();
            }

            MemberEntity parent = null;
            if (string.IsNullOrWhiteSpace(parentMobile))
            {
                return "请输入上级手机号码";
            }
            parent = members.Where(m => m.Mobile == parentMobile).FirstOrDefault();
            if (parent == null)
            {
                return "上级手机号码不存在，请重新输入";
            }

            MemberEntity member = new MemberEntity();
            member.ID = Guid.NewGuid();
            member.Mobile = mobile;
            member.MemberName = memberName;
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = DateTime.Now;
            member.ParentMemberID = parent.ID;
            member.MemberName = memberName;
            member.RoleID = GlobalConstants.InitialRoleID;
            member.Address = address;
            repo.Add(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            if (userRepo.FindBy(u => u.Account == member.Mobile).FirstOrDefault() != null)
            {
                return "该手机号码已经存在，请重新输入";
            }
            UserEntity user = new UserEntity();
            user.Account = member.Mobile;
            user.CreateBy = this.CurrentUser.ID;
            user.CreateOn = DateTime.Now;
            user.ID = Guid.NewGuid();
            user.MemberID = member.ID;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            userRepo.Add(user);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        public string UpdateHeighNumber(Guid memberId, string cardNo, string memberName, string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 修改代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateMember(AddMemberModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }

            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            MemberEntity parent = null;
            if (!string.IsNullOrWhiteSpace(model.ParentMobile))
            {
                long iParentMobile = 0;
                if (!long.TryParse(model.ParentMobile, out iParentMobile))
                {
                    throw new Exception("上级手机号码必须是数字");
                }
                if (model.ParentMobile.Length != 11)
                {
                    throw new Exception("上级手机号码长度必须是11位");
                }

                parent = repo.FindBy(p => p.Mobile == model.ParentMobile).FirstOrDefault();
                if (parent == null)
                {
                    throw new Exception("上级手机号码输入有误，请重新输入");
                }
                if (model.Mobile == model.ParentMobile)
                {
                    throw new Exception("上级手机号码不能和本人手机号码一样");
                }
            }

            var member = repo.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
                throw new Exception("代理ID传入有误，请联系管理员");
            if (!member.Mobile.Equals(model.Mobile))
            {
                if (repo.FindBy(p => p.Mobile == model.Mobile).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.RoleID = model.RoleId;
            member.ParentMemberID = parent == null ? Guid.Empty : parent.ID;

            member.Address = model.Address;
            repo.Update(member);
            Uow.Commit();
        }

        /// <summary>
        /// 修改子代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateChildMemberForAdmin(Member model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (model.ParentMember != null)
            {
                if (model.Mobile == model.ParentMember.Mobile)
                {
                    throw new Exception("上级电话号码不能和自己电话号码相同");
                }
            }

            var db = new Repository<MemberEntity>(Uow);

            Repository<RoleUpgradeEntity> ruRepo = new Repository<RoleUpgradeEntity>(Uow);

            var member = db.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
                throw new Exception("代理ID传入有误，请联系管理员");
            if (!member.Mobile.Equals(model.Mobile))
            {
                if (db.FindBy(p => p.Mobile == model.Mobile).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }

            if (model.RoleID != member.RoleID)
            {
                RoleUpgradeEntity ruEntity = new RoleUpgradeEntity();
                ruEntity.CreateBy = this.CurrentUser.ID;
                ruEntity.CreateOn = DateTime.Now;
                ruEntity.CurrentRoleId = model.RoleID;
                ruEntity.OriginalRoleId = member.RoleID;
                ruEntity.MemberId = member.ID;
                ruRepo.Add(ruEntity);
            }

            #region Check IdentityNo, make sure it has not been used.
            if (!string.Equals(member.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (db.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            //更新董事
            MemberEntity newMember = null;
            if (member.GeneralAvailable <= 0 && model.GeneralAvailable > 0)
            {
                //按总代更新
                newMember = CopyMember(member, (int)enmMemberRole.GeneralAgent);
            }
            else if (member.ProvinceAvailable <= 0 && model.ProvinceAvailable > 0)
            {
                //按省代更新
                newMember = CopyMember(member, (int)enmMemberRole.Province);
            }
            if (newMember != null)
            {
                UpdateDirector(new List<MemberEntity>() { newMember });
            }

            Guid parentId = Guid.Empty;
            var parentModel = db.FindBy(o => o.Mobile == model.ParentMember.Mobile).FirstOrDefault();
            if (parentModel != null)
            {
                parentId = parentModel.ID;
            }
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.RoleID = model.RoleID;
            if (parentId != Guid.Empty)
                member.ParentMemberID = parentId;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            member.ProvinceAvailable = model.ProvinceAvailable;
            member.GeneralAvailable = model.GeneralAvailable;
            member.TotalQuantity = model.TotalQuantity;
            member.CurrentRoleQuantity = model.CurrentRoleQuantity;
            db.Update(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.FindBy(u => u.MemberID == member.ID).FirstOrDefault();
            user.Account = member.Mobile;
            user.UpdateBy = this.CurrentUser.ID;
            user.UpdateOn = DateTime.Now;
            userRepo.Update(user);

            Uow.Commit();
        }

        /// <summary>
        /// 后台修改代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateChildMemberForAdmin1(Member model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (model.ParentMember != null)
            {
                if (model.Mobile == model.ParentMember.Mobile)
                {
                    throw new Exception("上级电话号码不能和自己电话号码相同");
                }
            }

            var db = new Repository<MemberEntity>(Uow);

            Repository<RoleUpgradeEntity> ruRepo = new Repository<RoleUpgradeEntity>(Uow);

            var member = db.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
                throw new Exception("代理ID传入有误，请联系管理员");
            if (!member.Mobile.Equals(model.Mobile))
            {
                if (db.FindBy(p => p.Mobile == model.Mobile).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }

            if (model.RoleID != member.RoleID)
            {
                RoleUpgradeEntity ruEntity = new RoleUpgradeEntity();
                ruEntity.CreateBy = this.CurrentUser.ID;
                ruEntity.CreateOn = DateTime.Now;
                ruEntity.CurrentRoleId = model.RoleID;
                ruEntity.OriginalRoleId = member.RoleID;
                ruEntity.MemberId = member.ID;
                ruRepo.Add(ruEntity);
            }

            #region Check IdentityNo, make sure it has not been used.
            if (!string.Equals(member.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (db.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            //更新董事
            MemberEntity newMember = null;
            if (member.GeneralAvailable <= 0 && model.GeneralAvailable > 0)
            {
                //按总代更新
                newMember = CopyMember(member, (int)enmMemberRole.GeneralAgent);
            }
            else if (member.ProvinceAvailable <= 0 && model.ProvinceAvailable > 0)
            {
                //按省代更新
                newMember = CopyMember(member, (int)enmMemberRole.Province);
            }
            if (newMember != null)
            {
                UpdateDirector(new List<MemberEntity>() { newMember });
            }

            Guid parentId = Guid.Empty;
            var parentModel = db.FindBy(o => o.Mobile == model.ParentMember.Mobile).FirstOrDefault();
            if (parentModel != null)
            {
                parentId = parentModel.ID;
            }
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.RoleID = model.RoleID;
            if (parentId != Guid.Empty)
                member.ParentMemberID = parentId;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            member.ProvinceAvailable = model.ProvinceAvailable;
            member.GeneralAvailable = model.GeneralAvailable;
            member.CurrentRoleAmount = model.CurrentRoleAmount;
            member.TotalAmount = model.TotalAmount; // 总金额
            db.Update(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.FindBy(u => u.MemberID == member.ID).FirstOrDefault();
            user.Account = member.Mobile;
            user.UpdateBy = this.CurrentUser.ID;
            user.UpdateOn = DateTime.Now;
            userRepo.Update(user);

            Uow.Commit();
        }

        /// <summary>
        /// 修改子代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateChildMember(AddMemberModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }

            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);

            var member = repo.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
                throw new Exception("代理ID传入有误，请联系管理员");
            if (!member.Mobile.Equals(model.Mobile))
            {
                if (repo.FindBy(p => p.Mobile == model.Mobile).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }

            #region Check IdentityNo: cannot be an existing no .
            if (!string.IsNullOrWhiteSpace(model.IdentityNo) && !string.Equals(member.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase))
            {
                if (repo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;

            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            repo.Update(member);
            Uow.Commit();
        }

        /// <summary>
        /// 修改当前角色库存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void UpdateCurrentRoleStock(AddMemberModel model)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);

            var member = repo.FindBy(p => p.ID == model.ID).FirstOrDefault();
            if (member == null)
                throw new Exception("代理ID传入有误，请联系管理员");

            member.CurrentRoleQuantity = model.CurrentRoleQuantity;
            member.TotalQuantity = model.TotalQuantity;
            repo.Update(member);
            Uow.Commit();
        }

        public List<MemberModel> GetMemberForUpdateStore(string cardNo, string name, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public List<MemberModel> GetDepartInfo(string cardNo)
        {
            throw new NotImplementedException();
        }


        public string CheckNewOrOldMember(string cardNo, int checkType)
        {
            throw new NotImplementedException();
        }

        public List<MemberModel> GetRecommendMemberLocation(string recommendCardNo, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }


        public string IDCardExists(string cardNo)
        {
            throw new NotImplementedException();
        }


        public string UpdateStore(string cardNo, string account)
        {
            throw new NotImplementedException();
        }



        public string GetMemberName(string mobile)
        {
            throw new NotImplementedException();
        }


        public List<MemberOrderModel> GetMemberOrder(string memberName, string mobile, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public void RemoveMember(Guid memberId)
        {
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);

            var member = memberRepo.FindBy(p => p.ID == memberId).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }
            //判断该代理是否已经有订单记录
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            var order = orderRepo.FindBy(o => o.MemberID == memberId).FirstOrDefault();
            if (order != null)
            {
                throw new Exception("该代理已经下过订单，不能删除");
            }
            //有下级代理，不能删除
            var hasChild = memberRepo.FindBy(o => o.ParentMemberID == memberId).Any();
            if (hasChild)
            {
                throw new Exception("该代理存在下级代理，不能删除");
            }
            var parentId = member.ParentMemberID;
            var roleId = member.RoleID;
            //Remove from members
            memberRepo.Remove(member);

            //remove from users
            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            userRepo.Remove(userRepo.FindBy(u => u.MemberID == memberId).FirstOrDefault());

            //上级库存加1
            if (roleId == (int)enmMemberRole.Agent1)
            {
                if (parentId.HasValue)
                {
                    if (memberRepo.FindBy(o => o.ID == parentId.Value).Any())
                    {
                        var parentMember = memberRepo.FindBy(o => o.ID == parentId.Value).Single();
                        parentMember.TotalQuantity += 1;
                        memberRepo.Update(parentMember);
                    }
                }

            }

            Uow.Commit();
        }

        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public void RemoveMember1(Guid memberId)
        {
            var memberDb = new Repository<MemberEntity>(Uow);

            var member = memberDb.FindBy(p => p.ID == memberId).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }
            //判断该代理是否已经有订单记录
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            var order = orderRepo.FindBy(o => o.MemberID == memberId).FirstOrDefault();
            if (order != null)
            {
                throw new Exception("该代理已经下过订单，不能删除");
            }
            //有下级代理，不能删除
            var hasChild = memberDb.FindBy(o => o.ParentMemberID == memberId).Any();
            if (hasChild)
            {
                throw new Exception("该代理存在下级代理，不能删除");
            }

            //删除代理
            memberDb.Remove(member);

            //删除用户
            var userDb = new Repository<UserEntity>(Uow);
            var userEntity = userDb.FindBy(u => u.MemberID == memberId).First();
            userDb.Remove(userEntity);

            Uow.Commit();
        }

        public void ResetPassword(Guid memberId)
        {
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);

            var member = memberRepo.FindBy(p => p.ID == memberId).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该用户在代理表中不存在，请联系管理员");
            }
            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.FindBy(u => u.MemberID == memberId).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("该用户不存在，请联系管理员");
            }
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            userRepo.Update(user);
            Uow.Commit();
        }

        /// <summary>
        /// 添加直属代理
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="memberName"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public void AddDirectChildMember(AddChildMemberModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }

            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("姓名必须输入");
            }

            if (string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                throw new Exception("身份证号码必须输入");
            }

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                throw new Exception("发货地址必须输入");
            }

            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            if (memberRepo.FindBy(m => m.Mobile == model.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("该手机号码已经存在，请重新输入");
            }


            #region Check IdentityNo: It cannot be an existing IdentityNo
            if (!string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (memberRepo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            #region check 上级库存
            var curUser = memberRepo.FindBy(o => o.ID == this.CurrentUser.MemberID).First();
            if (curUser.TotalQuantity < 1)
            {
                throw new Exception("您库存不够，不能添加代理");
            }
            #endregion

            #region 添加member表
            MemberEntity member = new MemberEntity();
            member.ID = Guid.NewGuid();
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = DateTime.Now;
            member.ParentMemberID = this.CurrentUser.MemberID;
            member.RoleID = (int)enmMemberRole.Agent1;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            memberRepo.Add(member);
            #endregion

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            if (userRepo.FindBy(u => u.Account == member.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("该手机号码已经存在于用户表中，请重新输入");
            }
            #region 添加user表
            UserEntity user = new UserEntity();
            user.Account = member.Mobile;
            user.CreateBy = this.CurrentUser.ID;
            user.CreateOn = DateTime.Now;
            user.ID = Guid.NewGuid();
            user.MemberID = member.ID;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            userRepo.Add(user);
            #endregion

            #region 上级库存减1
            curUser.TotalQuantity -= 1;
            memberRepo.Update(curUser);
            #endregion

            Uow.Commit();
        }

        /// <summary>
        /// add a member
        /// </summary>
        /// <param name="model">a member object</param>
        public void AddDirectChildMember1(AddUpdateMember model)
        {
            DateTime now = DateTime.Now;

            #region general validation
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }

            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("姓名必须输入");
            }

            if (string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                throw new Exception("身份证号码必须输入");
            }

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                throw new Exception("发货地址必须输入");
            }



            #endregion

            #region mobile should be unique in member table

            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            if (memberRepo.FindBy(m => m.Mobile == model.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("手机号码已被使用，请重新输入");
            }

            MemberEntity parent = null;
            if (!string.IsNullOrWhiteSpace(model.ParentMember.Mobile))
            {
                long iParentMobile = 0;
                if (!long.TryParse(model.ParentMember.Mobile, out iParentMobile))
                {
                    throw new Exception("上级手机号码必须是数字");
                }
                if (model.ParentMember.Mobile.Length != 11)
                {
                    throw new Exception("上级手机号码长度必须是11位");
                }

                parent = memberRepo.FindBy(p => p.Mobile == model.ParentMember.Mobile).FirstOrDefault();
                if (parent == null)
                {
                    throw new Exception("上级手机号码输入有误，请重新输入");
                }
                if (model.Mobile == model.ParentMember.Mobile)
                {
                    throw new Exception("上级手机号码不能和本人手机号码一样");
                }
            }

            #endregion

            #region identity number should be unique
            if (!string.IsNullOrWhiteSpace(model.IdentityNo))
            {
                if (memberRepo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已被使用，请重新输入");
                }
            }
            #endregion

            if (this.CurrentUser.RoleID <= (int)enmMemberRole.Customer)
            {
                throw new Exception("一级代理以上才能添加代理");
            }



            #region add to member table
            MemberEntity member = new MemberEntity();
            member.ID = Guid.NewGuid();
            member.Mobile = model.Mobile;
            member.MemberName = model.MemberName;
            member.CreateBy = this.CurrentUser.ID;
            member.CreateOn = now;
            member.ParentMemberID = this.CurrentUser.MemberID;
            member.RoleID = (int)enmMemberRole.Customer;
            member.Address = model.Address;
            member.IdentityNo = model.IdentityNo;
            if (parent != null)
            {
                member.ParentMemberID = parent.ID;
            }
            memberRepo.Add(member);
            #endregion



            #region mobile should be unique in user table
            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            if (userRepo.FindBy(u => u.Account == member.Mobile).FirstOrDefault() != null)
            {
                throw new Exception("手机号码已被使用，请重新输入");
            }
            #endregion

            #region add to user table
            UserEntity user = new UserEntity();
            user.Account = member.Mobile;
            user.CreateBy = this.CurrentUser.ID;
            user.CreateOn = now;
            user.ID = Guid.NewGuid();
            user.MemberID = member.ID;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            userRepo.Add(user);
            #endregion


            Uow.Commit();
        }

        /// <summary>
        /// get all products
        /// </summary>
        /// <returns></returns>
        public ProductSummary GetProducts()
        {
            var productDb = new Repository<ProductEntity>(Uow);
            var products = productDb.GetAll().ToList();
            var result = new ProductSummary();
            Mapper.CreateMap<ProductEntity, Product>();
            result.Products = Mapper.Map<IEnumerable<Product>>(products);
            return result;
        }

        public void AddOrder(IEnumerable<ProductMemberRole> products)
        {
            if (!products.Any(o => o.Quantity > 0))
            {
                throw new Exception("请输入购买数量");
            }
            DateTime now = DateTime.Now;

            var roleDb = new Repository<MemberRoleEntity>(Uow);
            Guid sendMemberId = Guid.Empty;
            var memberRole = roleDb.FindBy(mr => mr.ID == this.CurrentUser.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
                {
                    List<MemberEntity> members = db.Database.SqlQuery<MemberEntity>("EXEC [P_GetFirstHigherAgentMember] @MemberID", new SqlParameter("@MemberID", this.CurrentUser.MemberID)).ToList();
                    if (members != null && members.Any())
                    {
                        sendMemberId = members[0].ID;
                    }
                }

            }
            var productRoleDb = new Repository<ProductMemberRoleEntity>(Uow);
            foreach (var product in products.Where(o => o.Quantity > 0))
            {
                var productRole = productRoleDb.FindBy(o => o.MemberRoleID == this.CurrentUser.RoleID && o.ProductID == product.ProductID).Single();
                if (product.Quantity < productRole.OneTimeAmount)
                {
                    throw new Exception(string.Format("{0}单次进货{1}数量不能小于{2}件", productRole.MemberRole.RoleName, productRole.Product.Name, productRole.OneTimeAmount));
                }
            }

            #region add to order table



            var orderDb = new Repository<OrderEntity>(Uow);
            var orderEntity = new OrderEntity();
            orderEntity.ID = Guid.NewGuid();
            orderEntity.Status = (int)OrderStatus.CodeFull;
            orderEntity.CreateBy = this.CurrentUser.ID;
            orderEntity.CreateOn = now;
            orderEntity.Description = "";
            orderEntity.IsDeliverly = true;
            orderEntity.MemberID = this.CurrentUser.MemberID;
            orderEntity.OrderNo = GetOrderNo();
            orderEntity.SendDate = now;
            orderEntity.SendMemberID = sendMemberId;

            orderDb.Add(orderEntity);
            #endregion

            #region add to order details

            var orderDetailsDb = new Repository<OrderDetailsEntity>(Uow);
            OrderDetailsEntity orderDetailsEntity = null;
            foreach (var product in products.Where(o => o.Quantity > 0))
            {
                orderDetailsEntity = new OrderDetailsEntity();
                orderDetailsEntity.Status = (int)OrderDetailsStatus.CodeNotFull;
                orderDetailsEntity.CreateBy = this.CurrentUser.ID;
                orderDetailsEntity.CreateOn = now;
                orderDetailsEntity.ID = Guid.NewGuid();
                orderDetailsEntity.OrderID = orderEntity.ID;
                orderDetailsEntity.ProductID = product.ProductID;
                orderDetailsEntity.Quantity = product.Quantity;
                orderDetailsDb.Add(orderDetailsEntity);
            }

            #endregion
        }

        /// <summary>
        /// 获取自己角色下的产品信息
        /// </summary>
        /// <returns></returns>
        public ProductMemberRoleSummary GetProductMemberRole()
        {
            var productDb = new Repository<ProductMemberRoleEntity>(Uow);
            var products = productDb.FindBy(o => o.MemberRoleID == this.CurrentUser.RoleID).ToList();
            var result = new ProductMemberRoleSummary();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<ProductMemberRoleEntity, ProductMemberRole>();
            result.Products = Mapper.Map<IEnumerable<ProductMemberRole>>(products);
            return result;
        }

        /// <summary>
        /// 保存基本资料
        /// </summary>
        /// <param name="model"></param>
        public void SaveBaseInfo(BaseInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("代理姓名必须输入");
            }
            if (string.IsNullOrWhiteSpace(model.Address))
            {
                throw new Exception("发货地址必须输入");
            }
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);

            var member = memberRepo.FindBy(p => p.ID == this.CurrentUser.MemberID).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }

            if (!member.Mobile.Equals(model.Mobile.Trim()))
            {
                if (memberRepo.FindBy(p => p.Mobile == model.Mobile.Trim()).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }

            #region Check IdentityNo: cannot be an existing IdentityNo
            if (!string.IsNullOrWhiteSpace(model.IdentityNo) && !string.Equals(model.IdentityNo, member.IdentityNo, StringComparison.CurrentCultureIgnoreCase))
            {
                if (memberRepo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            member.MemberName = model.MemberName.Trim();
            member.Mobile = model.Mobile.Trim();
            member.Address = model.Address.Trim();
            member.IdentityNo = model.IdentityNo;
            memberRepo.Update(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.FindBy(u => u.ID == this.CurrentUser.ID).FirstOrDefault();
            user.UserName = model.MemberName.Trim();
            user.Account = model.Mobile.Trim();
            userRepo.Update(user);
            Uow.Commit();
        }

        public void SavePersonalInfo(MemberModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Mobile))
            {
                throw new Exception("手机号码必须输入");
            }
            long iMobile = 0;
            if (!long.TryParse(model.Mobile, out iMobile))
            {
                throw new Exception("手机号码必须是数字");
            }
            if (model.Mobile.Length != 11)
            {
                throw new Exception("手机号码长度必须是11位");
            }
            if (string.IsNullOrWhiteSpace(model.MemberName))
            {
                throw new Exception("代理姓名必须输入");
            }
            if (string.IsNullOrWhiteSpace(model.Address))
            {
                throw new Exception("发货地址必须输入");
            }
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);

            var member = memberRepo.FindBy(p => p.ID == this.CurrentUser.MemberID).FirstOrDefault();
            if (member == null)
            {
                throw new Exception("该代理不存在，请联系管理员");
            }

            if (!member.Mobile.Equals(model.Mobile.Trim()))
            {
                if (memberRepo.FindBy(p => p.Mobile == model.Mobile.Trim()).Any())
                {
                    throw new Exception("该手机号码已经存在，请重新输入");
                }
            }

            #region Check IdentityNo: cannot be an existing IdentityNo
            if (!string.IsNullOrWhiteSpace(model.IdentityNo) && !string.Equals(model.IdentityNo, member.IdentityNo, StringComparison.CurrentCultureIgnoreCase))
            {
                if (memberRepo.FindBy(m => string.Equals(m.IdentityNo, model.IdentityNo, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    throw new Exception("身份证号码已经存在");
                }
            }
            #endregion

            member.MemberName = model.MemberName.Trim();
            member.Mobile = model.Mobile.Trim();
            member.Address = model.Address.Trim();
            member.IdentityNo = model.IdentityNo;
            memberRepo.Update(member);

            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.FindBy(u => u.ID == this.CurrentUser.ID).FirstOrDefault();
            user.UserName = model.MemberName.Trim();
            user.Account = model.Mobile.Trim();
            userRepo.Update(user);
            Uow.Commit();
        }

        public MemberModel GetPersonalInfo()
        {
            Repository<UserEntity> repoUser = new Repository<UserEntity>(Uow);
            var user = repoUser.FindBy(m => m.MemberID == this.CurrentUser.MemberID).FirstOrDefault();
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var member = repo.FindBy(m => m.ID == this.CurrentUser.MemberID).FirstOrDefault();
            MemberModel model = new MemberModel();
            if (user.UserRoleID != (int)enmRoles.Admin &&
                user.UserRoleID != (int)enmRoles.All)
            {

                Repository<MemberRoleEntity> roleRepo = new Repository<MemberRoleEntity>(Uow);
                var role = roleRepo.FindBy(r => r.ID == member.RoleID).FirstOrDefault();
                int riseQuantity = Common.GetRiseQuantity(role.RoleRiseDescription, (int)member.CurrentRoleQuantity);
                if (member.DirectorCount > 0)
                {
                    if (member.DirectorCount == 2)
                    {
                        role.RoleName = "双" + role.RoleName;
                    }
                    else if (member.DirectorCount > 2)
                    {
                        role.RoleName = member.DirectorCount + role.RoleName;
                    }
                }
                model.MemberRoleName = role.RoleName;
                model.RiseQuantity = riseQuantity;
                model.TotalQuantity = member.TotalQuantity;

            }

            model.Address = member.Address;
            model.MemberName = member.MemberName;
            model.Mobile = member.Mobile;
            model.UserRoleId = user.UserRoleID;
            model.IdentityNo = member.IdentityNo;

            return model;
        }


        public BaseInfo GetPersonalInfo1()
        {
            Repository<UserEntity> repoUser = new Repository<UserEntity>(Uow);
            var user = repoUser.FindBy(m => m.MemberID == this.CurrentUser.MemberID).FirstOrDefault();
            Repository<MemberEntity> repo = new Repository<MemberEntity>(Uow);
            var member = repo.FindBy(m => m.ID == this.CurrentUser.MemberID).FirstOrDefault();
            BaseInfo model = new BaseInfo();
            if (user.UserRoleID != (int)enmRoles.Admin &&
                user.UserRoleID != (int)enmRoles.All)
            {

                Repository<MemberRoleEntity> roleRepo = new Repository<MemberRoleEntity>(Uow);
                var role = roleRepo.FindBy(r => r.ID == member.RoleID).FirstOrDefault();
                var scDb = new Repository<SystemConfigEntity>(Uow);
                var strUnitAmount = scDb.FindBy(o => o.Name == SystemSettingConstants.Price).First().ConfigValue;
                var unitAmount = decimal.Parse(strUnitAmount);

                // 原价升级金额
                var upgradeAmount = Common.GetUpgradeAmount(role.RoleRiseDescription, unitAmount, member.CurrentRoleAmount);
                if (member.DirectorCount > 0)
                {
                    if (member.DirectorCount == 2)
                    {
                        role.RoleName = "双" + role.RoleName;
                    }
                    else if (member.DirectorCount > 2)
                    {
                        role.RoleName = member.DirectorCount + role.RoleName;
                    }
                }
                Mapper.CreateMap<MemberRoleEntity, MemberRole>();
                model.MemberRole = Mapper.Map<MemberRoleEntity, MemberRole>(role);

                // 实际升级金额 = (原价升级金额 - 院级优惠金额) * 角色价格 / 单位价格
                model.UpgradeAmount = (upgradeAmount - member.CityMinus * unitAmount) * role.Price / unitAmount;

                // 保留两位小数
                model.UpgradeAmount = decimal.Round(model.UpgradeAmount, 2);

                // 优惠金额
                model.MinusAmount = member.CityMinus * unitAmount * role.Price / unitAmount;

                // 保留两位小数
                model.MinusAmount = decimal.Round(model.MinusAmount, 2);

            }

            model.Address = member.Address;
            model.MemberName = member.MemberName;
            model.Mobile = member.Mobile;
            model.UserRoleID = user.UserRoleID;
            model.IdentityNo = member.IdentityNo;

            return model;
        }


        public void UpdatePersonalOrder(AddPersonalOrderModel model)
        {
            Repository<OrderEntity> repo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<MemberRoleEntity> memberRoleRepo = new Repository<MemberRoleEntity>(Uow);

            var currentUserMember = memberRepo.FindBy(m => m.ID == this.CurrentUser.MemberID).FirstOrDefault();
            Guid sendMemberId = Guid.Empty;
            var memberRole = memberRoleRepo.FindBy(mr => mr.ID == currentUserMember.RoleID).FirstOrDefault();
            if (!memberRole.AllowedDirectOrder)
            {
                using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
                {
                    List<MemberEntity> members = db.Database.SqlQuery<MemberEntity>("EXEC [P_GetFirstHigherAgentMember] @MemberID", new SqlParameter("@MemberID", this.CurrentUser.MemberID)).ToList();
                    if (members != null && members.Any())
                    {
                        sendMemberId = members[0].ID;
                    }
                }
            }
            if (model.Quantity < memberRole.OneTimeAmount)
            {
                throw new Exception(string.Format("该用户单次进货数量不能小于{0}件", memberRole.OneTimeAmount));
            }
            var order = repo.FindBy(o => o.ID == model.ID).FirstOrDefault();
            if (order == null)
            {
                throw new Exception(string.Format("修改订单失败，订单信息不存在"));
            }
            if (order.IsDeliverly)
            {
                throw new Exception("该订单已经发货，不允许修改");
            }
            order.UpdateBy = this.CurrentUser.ID;
            order.UpdateOn = DateTime.Now;
            order.Quantity = model.Quantity;
            int totalAmount = 0;
            order.Description = Common.GetTotalAmountDescription(memberRole.Price, memberRole.RoleRiseDescription, model.Quantity, currentUserMember.RoleID, (int)currentUserMember.CurrentRoleQuantity, out totalAmount);
            order.Total = totalAmount;
            repo.Update(order);
            Uow.Commit();
        }

        public List<SubMemberModel> GetAllSubMembers(string mobileOrName, int levelId)
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                List<SubMemberModel> members = db.Database.SqlQuery<SubMemberModel>("EXEC [P_GetAllSubMembers] @MemberID, @MobileOrName, @LevelID", new SqlParameter("@MemberID", this.CurrentUser.MemberID), new SqlParameter("@MobileOrName", (mobileOrName ?? "")), new SqlParameter("@LevelID", levelId)).ToList();

                // 3层以下的代理都算作第3层
                if (members != null && members.Any())
                {
                    members.ForEach(o =>
                    {
                        if (o.LevelID > 3)
                        {
                            o.LevelID = 3;
                        }
                    });
                }
                return members;
            }
        }

        public SubMemberSummary GetAllSubMembers1(int page, int rows, string mobileOrName, int levelId)
        {
            SubMemberSummary model = new SubMemberSummary();
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                var members = db.Database.SqlQuery<SubMemberModel>("EXEC [P_GetAllSubMembers] @MemberID, @MobileOrName, @LevelID", new SqlParameter("@MemberID", this.CurrentUser.MemberID), new SqlParameter("@MobileOrName", (mobileOrName ?? "")), new SqlParameter("@LevelID", levelId)).ToList();

                // 3层以下的代理都算作第3层
                if (members != null && members.Any())
                {
                    members.ForEach(o =>
                    {
                        if (o.LevelID > 3)
                        {
                            o.LevelID = 3;
                        }
                    });

                    // 第1层代理人数
                    if (members.Any(o => o.LevelID == 1))
                    {
                        model.A = members.Where(o => o.LevelID == 1).Count();
                    }

                    // 第2层代理人数
                    if (members.Any(o => o.LevelID == 2))
                    {
                        model.B = members.Where(o => o.LevelID == 2).Count();
                    }

                    // 第3层即以下代理人数
                    if (members.Any(o => o.LevelID == 3))
                    {
                        model.C = members.Where(o => o.LevelID == 3).Count();
                    }

                    var totalCount = members.Count();
                    var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

                    model.Members = members.OrderBy(o => o.LevelID).Skip((page - 1) * rows).Take(rows).ToList();

                    CalculateRowNo(model, model.Members, page, rows, totalCount);

                }
            }

            return model;
        }

        public List<SubMemberModel> GetHighSubMembers(string mobileOrName)
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                List<SubMemberModel> members = db.Database.SqlQuery<SubMemberModel>("EXEC [P_GetHighSubMembers] @MemberID,@RoleID, @MobileOrName", new SqlParameter("@MemberID", this.CurrentUser.MemberID), new SqlParameter("@RoleID", this.CurrentUser.RoleID), new SqlParameter("@MobileOrName", (mobileOrName ?? ""))).ToList();
                return members;
            }
        }

        public SubMemberSummary GetHighSubMembers1(int page, int rows, string mobileOrName)
        {
            SubMemberSummary model = new SubMemberSummary();
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                List<SubMemberModel> members = db.Database.SqlQuery<SubMemberModel>("EXEC [P_GetHighSubMembers] @MemberID,@RoleID, @MobileOrName", new SqlParameter("@MemberID", this.CurrentUser.MemberID), new SqlParameter("@RoleID", this.CurrentUser.RoleID), new SqlParameter("@MobileOrName", (mobileOrName ?? ""))).ToList();
                var totalCount = members.Count();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

                model.Members = members.OrderBy(o => o.LevelID).Skip((page - 1) * rows).Take(rows).ToList();

                CalculateRowNo(model, model.Members, page, rows, totalCount);
            }
            return model;
        }


        public IEnumerable<MemberRoleModel> GetMemberRole()
        {
            Repository<MemberRoleEntity> repo = new Repository<MemberRoleEntity>(Uow);
            var roles = repo.GetAll();
            MemberRoleModel model = null;
            var models = new List<MemberRoleModel>();
            foreach (var role in roles)
            {
                model = new MemberRoleModel();
                model.Id = role.ID;
                model.Name = role.RoleName;
                models.Add(model);
            }
            return models;
        }

        public void ChangePassword(ChangePasswordModel model)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                throw new Exception("确认密码和新密码不一致");
            }
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var curUser = repo.FindBy(o => o.ID == this.CurrentUser.ID).FirstOrDefault();
            if (curUser.Password != (new UserEntity()).SwitchEncryptDecrypt(model.OldPassword))
            {
                throw new Exception("原始密码输入有误");
            }
            curUser.Password = (new UserEntity()).SwitchEncryptDecrypt(model.NewPassword);
            repo.Update(curUser);
            Uow.Commit();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        public void ChangePassword1(UserPassword model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new Exception("确认密码和新密码不一致");
            }
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var curUser = repo.FindBy(o => o.ID == this.CurrentUser.ID).FirstOrDefault();
            if (curUser.Password != (new UserEntity()).SwitchEncryptDecrypt(model.OldPassword))
            {
                throw new Exception("原始密码输入有误");
            }
            curUser.Password = (new UserEntity()).SwitchEncryptDecrypt(model.NewPassword);
            repo.Update(curUser);
            Uow.Commit();
        }

        /// <summary>
        /// 为每个代理计算总进货量
        /// </summary>
        public void CalculateTotalOrder()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_CalculateTotalCount]");
            }
        }

        /// <summary>
        /// 为省代和总代设置有效角色
        /// </summary>
        public void SetAgentValidRole()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_CalculateValidRole]");
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

        /// <summary>
        /// 查询所有有效的省代，总代，董事, 并更新ParentChild表
        /// </summary>
        public void UpdateParentChild()
        {
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            var validAgentList = memberRepo.FindBy(o => o.RoleID >= (int)enmMemberRole.Province && (o.ProvinceAvailable > 0 || o.GeneralAvailable > 0 || o.ValidRole > 0)).ToList();
            foreach (var validAgent in validAgentList)
            {
                UpdateParentChild(validAgent.ID, validAgent.RoleID);
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
                                    directorEntity.CreateBy = this.CurrentUser.ID;
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
                                    directorEntity.CreateBy = this.CurrentUser.ID;
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
        /// 获取市代升级数量
        /// </summary>
        /// <returns></returns>
        private List<CityAgent> GetCityAgent()
        {
            var scDb = new Repository<SystemConfigEntity>(Uow);
            var xml = scDb.FindBy(o => o.Name == SystemSettingConstants.CityAgent).First().ConfigValue;
            return Common.GetCityAgent(xml);
        }


        /// <summary>
        /// 更新市代数量
        /// </summary>
        /// <param name="memberList"></param>
        public void UpdateCityAgentCount(List<MemberEntity> memberList)
        {
            //更新上下级关系表
            UpdateParentChild(memberList);

            var memberDb = new Repository<MemberEntity>(Uow);
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
            if (parentRepo.FindBy(o => allParents.Any(m => m == o.ChildMemberID) && o.CityAgentCount > 0).Any())
            {
                // 查找所有市级代理数量大于0的记录
                var parentList = parentRepo.FindBy(o => allParents.Any(m => m == o.ParentMemberID) && o.CityAgentCount > 0).GroupBy(o => o.ParentMemberID);
                MemberEntity member = null;
                List<MemberEntity> result = new List<MemberEntity>();
                int parentCount = 0;
                int cityMinus = 0;
                foreach (var parent in parentList)
                {
                    parentCount = parent.Count();
                    List<CityAgent> cityList = GetCityAgent();
                    cityList = cityList.OrderByDescending(o => o.Count).ToList();
                    foreach (var city in cityList)
                    {
                        if (parentCount >= city.Count)
                        {
                            cityMinus = city.Minus;
                            break;
                        }
                    }

                    // 查找省级以下的代理，更新市代升级数量
                    if (cityMinus > 0)
                    {
                        if (memberDb.FindBy(o => o.ID == parent.Key && o.RoleID < (int)enmMemberRole.Province).Any())
                        {
                            member = memberDb.FindBy(o => o.ID == parent.Key && o.RoleID < (int)enmMemberRole.Province).First();
                            member.CityMinus = cityMinus;
                            memberDb.Update(member);
                        }
                    }

                }
                Uow.Commit();
            }
        }

        /// <summary>
        /// 更新董事
        /// </summary>
        public void UpdateDirector()
        {
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Repository<ParentChildEntity> parentRepo = new Repository<ParentChildEntity>(Uow);
            var parentList = parentRepo.GetAll().GroupBy(o => o.ParentMemberID).ToList();
            MemberEntity member = null;
            int childListCount = 0;
            bool bUpdate = false;
            List<MemberEntity> result = new List<MemberEntity>();
            foreach (var parent in parentList)
            {
                if (parent.Count() >= GlobalConstants.ProvinceCountForDirector)
                {
                    bUpdate = true;
                }
                else if (parent.Count() == GlobalConstants.GeneralCountForDirector)
                {
                    childListCount = parentRepo.FindBy(o => o.ParentMemberID == parent.Key && o.GeneralAgentCount > 0).ToList().Count();
                    if (childListCount == GlobalConstants.GeneralCountForDirector)
                    {
                        bUpdate = true;
                    }
                }
                else
                {
                    bUpdate = false;
                }
                if (bUpdate)
                {
                    if (memberRepo.FindBy(o => o.ID == parent.Key && o.RoleID != (int)enmMemberRole.Director).Any())
                    {
                        member = memberRepo.FindBy(o => o.ID == parent.Key).First();
                        member.RoleID = (int)enmMemberRole.Director;
                        memberRepo.Update(member);
                        result.Add(member);
                    }
                }
            }
            Uow.Commit();
            if (result.Any())
            {
                UpdateParentChild(result);
                UpdateDirector();
            }
        }

        /// <summary>
        /// 清空上下级关系表
        /// </summary>
        public void CleanUpParentChild()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_CleanUpParentChild]");
            }
        }

        /// <summary>
        /// 更新订单上面三个总代
        /// </summary>
        public void UpdateOrderGeneral()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_UpdateAvailabledGeneralOrder]");
            }
        }

        /// <summary>
        /// 更新董事日期
        /// </summary>
        public void UpdateDirectorDate()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {

                db.Database.ExecuteSqlCommand("EXEC [P_UpdateDirectorDate]");
            }
        }

        /// <summary>
        /// 更新董事日期
        /// </summary>
        public void UpdateAwardDirector()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                db.Database.ExecuteSqlCommand("EXEC [P_UpdateOrderForDirector]");
            }
        }

        /// <summary>
        /// 更新订单日期
        /// </summary>
        public void UpdateOrderSendDate()
        {
            using (SimpleWebUnitOfWork db = new SimpleWebUnitOfWork())
            {
                db.Database.ExecuteSqlCommand("EXEC [P_UpdateOrderSendDate]");
            }
        }

        private List<Member503020> Calculate50(List<Member503020> member503020List, DateTime? dateFrom, DateTime? dateTo)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Member503020 member503020 = null;
            var orderListForGeneral1 = orderRepo.FindBy(o => o.GeneralAgent1ID != null && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            if (dateFrom.HasValue)
            {
                DateTime dtFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, 0, 0, 0);
                orderListForGeneral1 = orderListForGeneral1.Where(o => o.CreateOn >= dtFrom);
            }
            if (dateTo.HasValue)
            {
                DateTime dtTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
                orderListForGeneral1 = orderListForGeneral1.Where(o => o.CreateOn <= dtTo);
            }
            if (orderListForGeneral1 != null && orderListForGeneral1.Any())
            {
                var general1List = orderListForGeneral1.GroupBy(o => o.GeneralAgent1ID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();

                if (memberRepo.FindBy(o => general1List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general1List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        if (!member503020List.Any(o => o.ID == member.ID))
                        {
                            member503020 = new Member503020();
                            member503020.ID = member.ID;
                            member503020.RoleId = member.RoleID;
                            member503020.MemberName = member.MemberName;
                            member503020.Mobile = member.Mobile;
                            member503020.Count50 += general1List.Where(o => o.key == member.ID).First().sum;
                            member503020List.Add(member503020);
                        }
                        else
                        {
                            member503020 = member503020List.Where(o => o.ID == member.ID).First();
                            member503020.Count50 += general1List.Where(o => o.key == member.ID).First().sum;
                        }
                    }
                }
            }
            return member503020List;
        }

        private List<Member503020> Calculate30(List<Member503020> member503020List, DateTime? dateFrom, DateTime? dateTo)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Member503020 member503020 = null;
            var orderListForGeneral2 = orderRepo.FindBy(o => o.GeneralAgent2ID != null && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            if (dateFrom.HasValue)
            {
                DateTime dtFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, 0, 0, 0);
                orderListForGeneral2 = orderListForGeneral2.Where(o => o.CreateOn >= dtFrom);
            }
            if (dateTo.HasValue)
            {
                DateTime dtTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
                orderListForGeneral2 = orderListForGeneral2.Where(o => o.CreateOn <= dtTo);
            }
            if (orderListForGeneral2 != null && orderListForGeneral2.Any())
            {
                var general2List = orderListForGeneral2.GroupBy(o => o.GeneralAgent2ID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();

                if (memberRepo.FindBy(o => general2List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general2List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        if (!member503020List.Any(o => o.ID == member.ID))
                        {
                            member503020 = new Member503020();
                            member503020.ID = member.ID;
                            member503020.RoleId = member.RoleID;
                            member503020.MemberName = member.MemberName;
                            member503020.Mobile = member.Mobile;
                            member503020.Count30 += general2List.Where(o => o.key == member.ID).First().sum;
                            member503020List.Add(member503020);
                        }
                        else
                        {
                            member503020 = member503020List.Where(o => o.ID == member.ID).First();
                            member503020.Count30 += general2List.Where(o => o.key == member.ID).First().sum;
                        }
                    }
                }
            }
            return member503020List;
        }

        private List<Member503020> Calculate20(List<Member503020> member503020List, DateTime? dateFrom, DateTime? dateTo)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            Member503020 member503020 = null;
            var orderListForGeneral3 = orderRepo.FindBy(o => o.GeneralAgent3ID != null && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            if (dateFrom.HasValue)
            {
                DateTime dtFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, 0, 0, 0);
                orderListForGeneral3 = orderListForGeneral3.Where(o => o.CreateOn >= dtFrom);
            }
            if (dateTo.HasValue)
            {
                DateTime dtTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
                orderListForGeneral3 = orderListForGeneral3.Where(o => o.CreateOn <= dtTo);
            }
            if (orderListForGeneral3 != null && orderListForGeneral3.Any())
            {
                var general3List = orderListForGeneral3.GroupBy(o => o.GeneralAgent3ID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();

                if (memberRepo.FindBy(o => general3List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general3List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        if (!member503020List.Any(o => o.ID == member.ID))
                        {
                            member503020 = new Member503020();
                            member503020.ID = member.ID;
                            member503020.RoleId = member.RoleID;
                            member503020.MemberName = member.MemberName;
                            member503020.Mobile = member.Mobile;
                            member503020.Count20 += general3List.Where(o => o.key == member.ID).First().sum;
                            member503020List.Add(member503020);
                        }
                        else
                        {
                            member503020 = member503020List.Where(o => o.ID == member.ID).First();
                            member503020.Count20 += general3List.Where(o => o.key == member.ID).First().sum;
                        }
                    }
                }
            }
            return member503020List;
        }

        /// <summary>
        /// 计算50，30，20
        /// </summary>
        public List<Member503020> Calculate503020(string mobileOrName, DateTime? dateFrom, DateTime? dateTo)
        {
            List<Member503020> result = new List<Member503020>();
            Calculate50(result, dateFrom, dateTo);
            Calculate30(result, dateFrom, dateTo);
            Calculate20(result, dateFrom, dateTo);
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                result = result.Where(o => string.Equals(o.MemberName, mobileOrName, StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(o.Mobile, mobileOrName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            return result;
        }

        private List<DetailsModel> Details(IEnumerable<OrderEntity> orderList, DateTime? dateFrom, DateTime? dateTo)
        {
            List<DetailsModel> result = new List<DetailsModel>();
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            if (dateFrom.HasValue)
            {
                DateTime dtFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, 0, 0, 0);
                orderList = orderList.Where(o => o.CreateOn >= dtFrom);
            }
            if (dateTo.HasValue)
            {
                DateTime dtTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
                orderList = orderList.Where(o => o.CreateOn <= dtTo);
            }
            if (orderList != null && orderList.Any())
            {
                var memberList = memberRepo.FindBy(o => orderList.ToList().Any(order => order.MemberID == o.ID));
                DetailsModel model = null;
                foreach (var order in orderList)
                {
                    model = new DetailsModel();
                    model.CreateOn = order.CreateOn.Value;
                    model.MemberName = memberList.First(o => o.ID == order.MemberID).MemberName;
                    model.Mobile = memberList.First(o => o.ID == order.MemberID).Mobile;
                    model.Quantity = order.Quantity;
                    model.RoleId = memberList.First(o => o.ID == order.MemberID).RoleID;
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// 50-30-20订单明细
        /// </summary>
        public List<DetailsModel> Details503020(int detailsType, Guid memberId, DateTime? dateFrom, DateTime? dateTo)
        {
            List<DetailsModel> result = new List<DetailsModel>();
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);

            switch (detailsType)
            {
                case (int)enmDetailsType.type50:
                    {
                        var orderList = orderRepo.FindBy(o => o.GeneralAgent1ID == memberId && o.IsDeliverly && o.SendMemberID == Guid.Empty).ToList();
                        result = Details(orderList, dateFrom, dateTo);
                        break;
                    }
                case (int)enmDetailsType.type30:
                    {
                        var orderList = orderRepo.FindBy(o => o.GeneralAgent2ID == memberId && o.IsDeliverly && o.SendMemberID == Guid.Empty).ToList();
                        result = Details(orderList, dateFrom, dateTo);
                        break;
                    }
                case (int)enmDetailsType.type20:
                    {
                        var orderList = orderRepo.FindBy(o => o.GeneralAgent3ID == memberId && o.IsDeliverly && o.SendMemberID == Guid.Empty).ToList();
                        result = Details(orderList, dateFrom, dateTo);
                        break;
                    }
                case (int)enmDetailsType.typeSelf:
                    {
                        var orderList = orderRepo.FindBy(o => o.DirectorID == memberId && o.IsDeliverly && o.SendMemberID == Guid.Empty).ToList();
                        result = Details(orderList, dateFrom, dateTo);
                        break;
                    }
                case (int)enmDetailsType.typeCompany:
                    {
                        var orderList = orderRepo.FindBy(o => o.IsDeliverly && o.SendMemberID == Guid.Empty).ToList();
                        result = Details(orderList, dateFrom, dateTo);
                        break;
                    }
                default:
                    break;
            }
            return result;
        }

        private void CalculateDirectorSelf(List<DirectorBonusModel> directorBonusList, IEnumerable<MemberEntity> memberList, string mobileOrName, DateTime dateFrom, DateTime dateTo)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            if (memberList.Any())
            {
                if (!string.IsNullOrWhiteSpace(mobileOrName))
                {
                    memberList = memberList.Where(o => string.Equals(o.Mobile, mobileOrName, StringComparison.CurrentCultureIgnoreCase) ||
                        string.Equals(o.MemberName, mobileOrName, StringComparison.CurrentCultureIgnoreCase)).ToList();
                }
                if (!memberList.Any())
                {
                    return;
                }
            }
            var orderList = orderRepo.FindBy(o => o.CreateOn >= dateFrom && o.CreateOn < dateTo && o.SendMemberID == Guid.Empty && o.IsDeliverly).ToList();

            DirectorBonusModel director = null;
            decimal sumQuantity = 0;
            foreach (var member in memberList)
            {
                director = null;
                sumQuantity = 0;
                if (orderList.Any(o => o.DirectorID == member.ID))
                {
                    sumQuantity = orderList.Where(o => o.DirectorID == member.ID).Sum(o => o.Quantity);
                }
                if (directorBonusList.Any(o => o.ID == member.ID))
                {
                    director = directorBonusList.First(o => o.ID == member.ID);
                    director.SelfBonusStr = string.Format("{0}套 × 30元 = {1}元", sumQuantity, sumQuantity * 30);
                    director.SelfBonus = sumQuantity * 30;
                    director.ID = member.ID;
                    director.MemberName = member.MemberName;
                    director.Mobile = member.Mobile;
                    director.RoleId = member.RoleID;
                    director.Total = director.SelfBonus + director.CompanyBonus;
                }
                else
                {
                    director = new DirectorBonusModel();
                    director.SelfBonusStr = string.Format("{0}套 × 30元 = {1}元", sumQuantity, sumQuantity * 30);
                    director.SelfBonus = sumQuantity * 30;
                    director.ID = member.ID;
                    director.MemberName = member.MemberName;
                    director.Mobile = member.Mobile;
                    director.RoleId = member.RoleID;
                    director.Total = director.SelfBonus + director.CompanyBonus;
                    directorBonusList.Add(director);
                }
            }
        }

        private void CalculateDirectorCompany(List<DirectorBonusModel> directorBonusList, IEnumerable<MemberEntity> memberList, string mobileOrName, DateTime dateFrom, DateTime dateTo)
        {
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<DirectorEntity> directorRepo = new Repository<DirectorEntity>(Uow);
            int totalBounsCount = directorRepo.FindBy(o => memberList.Any(m => m.ID == o.MemberID) && o.CreateOn < dateFrom).Count();
            var directorList = directorRepo.FindBy(o => o.CreateOn < dateFrom).ToList();
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                memberList = memberList.Where(o => string.Equals(o.Mobile, mobileOrName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(o.MemberName, mobileOrName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (!memberList.Any())
            {
                return;
            }
            var orderList = orderRepo.FindBy(o => o.CreateOn >= dateFrom && o.CreateOn < dateTo && o.SendMemberID == Guid.Empty && o.IsDeliverly).ToList();
            decimal totalQuantity = 0;
            if (orderList.Any())
            {
                totalQuantity = orderList.Sum(o => o.Quantity);
            }

            decimal perDirector = 0;
            if (totalQuantity > 0)
            {
                perDirector = totalQuantity * 30 / totalBounsCount;
            }


            DirectorBonusModel director = null;
            int bonusCount = 0;
            foreach (var member in memberList)
            {
                director = null;
                bonusCount = directorList.Where(o => o.MemberID == member.ID).Count();
                if (directorBonusList.Any(o => o.ID == member.ID))
                {
                    director = directorBonusList.First(o => o.ID == member.ID);
                    director.CompanyBonusStr = string.Format("{0}套 × 30元 ÷ {1}份董事 * {2}份 = {3}元", totalQuantity, totalBounsCount, bonusCount, perDirector * bonusCount);
                    director.CompanyBonus = perDirector * bonusCount;
                    director.ID = member.ID;
                    director.MemberName = member.MemberName;
                    director.Mobile = member.Mobile;
                    director.RoleId = member.RoleID;
                    director.Total = director.SelfBonus + director.CompanyBonus;
                    director.DirectorCount = member.DirectorCount;
                    director.BonusCount = bonusCount;
                }
                else
                {
                    director = new DirectorBonusModel();
                    director.CompanyBonusStr = string.Format("{0}套 × 30元 ÷ {1}份董事 * {2}份 = {3}元", totalQuantity, totalBounsCount, bonusCount, perDirector * bonusCount);
                    director.CompanyBonus = perDirector * bonusCount;
                    director.ID = member.ID;
                    director.MemberName = member.MemberName;
                    director.Mobile = member.Mobile;
                    director.RoleId = member.RoleID;
                    director.Total = director.SelfBonus + director.CompanyBonus;
                    director.DirectorCount = member.DirectorCount;
                    director.BonusCount = bonusCount;
                    directorBonusList.Add(director);
                }
            }
        }

        /// <summary>
        /// 董事分红份数
        /// </summary>
        /// <param name="yearMonth"></param>
        /// <returns></returns>
        public int CalcualteCompanyBonusMemberCount(string yearMonth)
        {
            int result = 0;
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            int year = int.Parse(yearMonth.Substring(0, 4));
            int month = int.Parse(yearMonth.Substring(4, 2));
            DateTime dateTo = new DateTime(year, month, 1);
            var memberList = memberRepo.FindBy(o => o.DirectorDate < dateTo && o.RoleID == (int)enmMemberRole.Director).ToList();
            if (memberList.Any())
            {
                Repository<DirectorEntity> directorRepo = new Repository<DirectorEntity>(Uow);
                result = directorRepo.FindBy(o => memberList.Any(m => m.ID == o.MemberID) && o.CreateOn < dateTo).Count();
            }
            return result;
        }
        /// <summary>
        /// 计算董事分红
        /// </summary>
        public List<DirectorBonusModel> CalculateDirectorBonus(string mobileOrName, string yearMonth)
        {
            int year = int.Parse(yearMonth.Substring(0, 4));
            int month = int.Parse(yearMonth.Substring(4, 2));
            DateTime date = new DateTime(year, month, 1);
            List<DirectorBonusModel> result = new List<DirectorBonusModel>();
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            var memberList = memberRepo.FindBy(o => o.DirectorDate < date && o.RoleID == (int)enmMemberRole.Director);
            if (!memberList.Any())
            {
                return result;
            }
            CalculateDirectorCompany(result, memberList.ToList(), mobileOrName, date, date.AddMonths(1));
            CalculateDirectorSelf(result, memberList.ToList(), mobileOrName, date, date.AddMonths(1));

            return result;
        }

        /// <summary>
        /// 我的购物车
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ShoppingCartSummary ShoppingCart(int page, int rows)
        {
            var result = new ShoppingCartSummary();


            Expression<Func<ShoppingCartEntity, bool>> whereExp = o => o.MemberID == this.CurrentUser.MemberID;

            var totalCount = shoppingCartDb.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

            var shoppingCarts = shoppingCartDb.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * rows).Take(rows).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<ShoppingCartEntity, ShoppingCart>();



            var roleDb = new Repository<MemberRoleEntity>(Uow);
            var role = roleDb.FindBy(o => o.ID == this.CurrentUser.RoleID).Single();

            if (shoppingCarts.Any())
            {
                decimal total = 0m;
                foreach (var cart in shoppingCarts)
                {
                    if (cart.Product == null)
                    {
                        cart.Product = ProductDb.FindBy(o => o.ID == cart.ProductID).First();
                    }
                    total += cart.Product.Price * cart.Quantity;
                }
                var settingDb = new Repository<SystemConfigEntity>(Uow);
                var price = settingDb.FindBy(o => o.Name == SystemSettingConstants.Price).Single();
                var totalQuantity = total / decimal.Parse(price.ConfigValue);
                totalQuantity = decimal.Round(totalQuantity, 6);

                decimal totalAmount = 0m;
                result.Description = Common.GetTotalAmountDescription1(role.Price, role.RoleRiseDescription, totalQuantity, role.ID, this.CurrentUser.CurrentRoleQuantity, out totalAmount);

            }
            result.ShoppingCarts = Mapper.Map<IEnumerable<ShoppingCart>>(shoppingCarts);
            CalculateRowNo(result, result.ShoppingCarts, page, rows, totalCount);

            return result;
        }

        public void AddUpdateShoppingCart(ShoppingCart model)
        {
            var db = new Repository<ShoppingCartEntity>(Uow);

            if (model.ID != Guid.Empty)
            {
                //修改产品
                var entity = db.FindBy(o => o.ID == model.ID).Single();
                entity.ProductID = model.ProductID;
                entity.Quantity = model.Quantity;
                entity.UpdateBy = this.CurrentUser.ID;
                entity.UpdateOn = DateTime.Now;
                db.Update(entity);
            }
            else
            {
                var entity = db.FindBy(o => o.ProductID == model.ProductID && o.MemberID == this.CurrentUser.MemberID).SingleOrDefault();
                if (entity != null)
                {
                    //在原来产品上添加数量
                    entity.Quantity += model.Quantity;
                    entity.UpdateBy = this.CurrentUser.ID;
                    entity.UpdateOn = DateTime.Now;
                    db.Update(entity);
                }
                else
                {
                    //添加新产品
                    entity = new ShoppingCartEntity();
                    entity.ID = Guid.NewGuid();
                    entity.CreateBy = this.CurrentUser.ID;
                    entity.CreateOn = DateTime.Now;
                    entity.MemberID = this.CurrentUser.MemberID;
                    entity.ProductID = model.ProductID;
                    entity.Quantity = model.Quantity;
                    db.Add(entity);
                }
            }

            Uow.Commit();
        }

        public void RemoveShoppingCart(Guid ID)
        {
            var db = new Repository<ShoppingCartEntity>(Uow);
            var entity = db.FindBy(o => o.ID == ID).Single();
            db.Remove(entity);
            Uow.Commit();
        }

        public void AddOrder1()
        {
            var db = new Repository<ShoppingCartEntity>(Uow);
            var list = db.FindBy(o => o.MemberID == this.CurrentUser.MemberID).ToList();
            if (!list.Any())
            {
                throw new Exception("您还没有购买产品，不能提交订单");
            }

            var createBy = this.CurrentUser.ID;
            var createOn = DateTime.Now;

            Guid sendMemberId = Guid.Empty;
            var roleDb = new Repository<MemberRoleEntity>(Uow);
            var role = roleDb.FindBy(o => o.ID == this.CurrentUser.RoleID).First();
            if (!role.AllowedDirectOrder)
            {
                using (SimpleWebUnitOfWork myDb = new SimpleWebUnitOfWork())
                {
                    List<MemberEntity> members = myDb.Database.SqlQuery<MemberEntity>("EXEC [P_GetFirstHigherAgentMember] @MemberID", new SqlParameter("@MemberID", this.CurrentUser.MemberID)).ToList();
                    if (members != null && members.Any())
                    {
                        sendMemberId = members[0].ID;
                    }
                }

            }


            var orderNo = "DD" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var orderDb = new Repository<OrderEntity>(Uow);
            OrderEntity orderEntity = new OrderEntity();
            orderEntity.CreateBy = createBy;
            orderEntity.CreateOn = createOn;
            orderEntity.ID = Guid.NewGuid();
            orderEntity.IsDeliverly = false;
            orderEntity.MemberID = this.CurrentUser.MemberID;
            orderEntity.OrderNo = orderNo;
            orderEntity.SendMemberID = sendMemberId;
            if (this.CurrentUser.RoleID < (int)enmMemberRole.GeneralAgent)
            {
                orderEntity.FinancialStatus = (int)OrderFinancialStatus.Paid;
            }

            int quantity = 0;
            decimal total = 0m;
            foreach (var cart in list)
            {
                total += cart.Product.Price * cart.Quantity;
                quantity += cart.Quantity;
            }
            var settingDb = new Repository<SystemConfigEntity>(Uow);
            var price = settingDb.FindBy(o => o.Name == SystemSettingConstants.Price).Single();
            var totalQuantity = total / decimal.Parse(price.ConfigValue);
            totalQuantity = decimal.Round(totalQuantity, 6);

            decimal totalAmount = 0m;
            orderEntity.Description = Common.GetTotalAmountDescription1(role.Price, role.RoleRiseDescription, totalQuantity, role.ID, this.CurrentUser.CurrentRoleQuantity, out totalAmount);

            if (totalAmount < role.OneTimeAmount)
            {
                throw new Exception(string.Format("{0}一次进货不能少于{1}元", role.RoleName, role.OneTimeAmount));
            }


            orderEntity.Total = totalAmount;
            orderEntity.Quantity = totalQuantity;

            //如果有产品需要添加唯一识别码，则该订单需要添加唯一识别码
            if (list.Any(o => o.Product.HasIdentityCode == true))
            {
                orderEntity.Status = (int)OrderStatus.CodeNotFull;
            }

            orderDb.Add(orderEntity);

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            OrderDetailsEntity odEntity = null;
            foreach (var entity in list)
            {
                odEntity = new OrderDetailsEntity();
                odEntity.CreateBy = createBy;
                odEntity.CreateOn = createOn;
                odEntity.ID = Guid.NewGuid();
                odEntity.ProductID = entity.ProductID;
                odEntity.OrderID = orderEntity.ID;
                odEntity.Quantity = entity.Quantity;
                //是否需要登记唯一识别码
                if (entity.Product.HasIdentityCode)
                {
                    odEntity.Status = (int)OrderDetailsStatus.CodeNotFull;
                }
                else
                {
                    odEntity.Status = (int)OrderDetailsStatus.NoCode;
                }
                odDb.Add(odEntity);

                //remove shopping cart
                db.Remove(entity);
            }

            Uow.Commit();
        }

        public OrderDetailsSummary OrderDetails(Guid orderId, int page, int rows)
        {
            var result = new OrderDetailsSummary();

            var db = new Repository<OrderDetailsEntity>(Uow);

            Expression<Func<OrderDetailsEntity, bool>> whereExp = o => o.OrderID == orderId;

            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

            var orderDetails = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * rows).Take(rows).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<OrderDetailsEntity, OrderDetails>();

            result.OrderDetails = Mapper.Map<IEnumerable<OrderDetails>>(orderDetails);
            CalculateRowNo(result, result.OrderDetails, page, rows, totalCount);

            if (!result.OrderDetails.Any()) return result;

            var orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == orderId).First();

            if (!order.IsDeliverly)
            {
                //如果没有订单明细，则删除该订单
                if (!result.OrderDetails.Any())
                {
                    //删除订单
                    orderDb.Remove(order);
                    Uow.Commit();
                    return result;
                }
            }


            result.OrderNo = order.OrderNo;

            var roleDb = new Repository<MemberRoleEntity>(Uow);
            //订单所有者的角色
            var role = roleDb.FindBy(o => o.ID == order.Member.RoleID).First();

            //var allDetails = db.Find(whereExp, o => o.CreateOn).ToList();
            //if (allDetails.Any())
            //{
            //    decimal total = 0m;
            //    foreach (var detail in allDetails)
            //    {
            //        total += detail.Product.Price * detail.Quantity;
            //    }
            //    var settingDb = new Repository<SystemConfigEntity>(uow);
            //    var price = settingDb.FindBy(o => o.Name == "Price").Single();
            //    var totalQuantity = total / decimal.Parse(price.ConfigValue);
            //    totalQuantity = decimal.Round(totalQuantity, 6);

            //    decimal totalAmount = 0m;
            //    result.Description = Common.GetTotalAmountDescription1(role.Price, role.RoleRiseDescription, totalQuantity, role.ID, this.CurrentUser.CurrentRoleQuantity, out totalAmount);
            //    result.TotalAmount = totalAmount;

            //}

            result.Description = order.Description;
            result.TotalAmount = order.Total;

            return result;
        }


        /// <summary>
        /// 更新订单明细
        /// </summary>
        /// <param name="model"></param>
        public void AddUpdateOrderDetails(OrderDetails model)
        {
            var db = new Repository<OrderDetailsEntity>(Uow);

            var currentUserID = this.CurrentUser.ID;
            var now = DateTime.Now;

            bool HasIdentityCode = false;

            var proDb = new Repository<ProductEntity>(Uow);

            //需要登记唯一识别码
            if (proDb.FindBy(o => o.ID == model.ProductID).Any(o => o.HasIdentityCode == true))
            {
                HasIdentityCode = true;
            }

            if (model.ID != Guid.Empty)
            {

                //更新
                var entity = db.FindBy(o => o.ID == model.ID).Single();
                entity.ProductID = model.ProductID;
                entity.Quantity = model.Quantity;
                entity.UpdateBy = currentUserID;
                entity.UpdateOn = now;
                //是否需要登记唯一识别码
                if (HasIdentityCode)
                {
                    entity.Status = (int)OrderDetailsStatus.CodeNotFull;
                }
                else
                {
                    entity.Status = (int)OrderDetailsStatus.NoCode;
                }
                db.Update(entity);

            }
            else
            {
                var entity = db.FindBy(o => o.ProductID == model.ProductID && o.OrderID == model.OrderID).FirstOrDefault();
                if (entity != null)
                {
                    //累加
                    entity.Quantity += model.Quantity;
                    entity.UpdateBy = currentUserID;
                    entity.UpdateOn = now;
                    db.Update(entity);

                }
                else
                {
                    //新加
                    entity = new OrderDetailsEntity();
                    entity.ID = Guid.NewGuid();
                    entity.CreateBy = currentUserID;
                    entity.CreateOn = now;
                    entity.ProductID = model.ProductID;
                    entity.Quantity = model.Quantity;
                    entity.OrderID = model.OrderID;
                    //是否需要登记唯一识别码
                    if (HasIdentityCode)
                    {
                        entity.Status = (int)OrderDetailsStatus.CodeNotFull;
                    }
                    else
                    {
                        entity.Status = (int)OrderDetailsStatus.NoCode;
                    }
                    db.Add(entity);


                }

            }


            var orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == model.OrderID).First();

            if (!HasIdentityCode)
            {
                //需要登记唯一识别码
                if (db.FindBy(o => o.OrderID == model.OrderID).ToList().Any(o => o.Product.HasIdentityCode == true))
                {
                    HasIdentityCode = true;
                }
            }

            if (HasIdentityCode)
            {
                order.Status = (int)OrderStatus.CodeNotFull;
            }


            orderDb.Update(order);


            Uow.Commit();

            UpdateOrder(order.ID);
        }

        /// <summary>
        /// 修改订单明细之后更新订单
        /// </summary>
        /// <param name="orderId"></param>
        private void UpdateOrder(Guid orderId)
        {
            var orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == orderId).First();

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var list = odDb.FindBy(o => o.OrderID == orderId).ToList();

            var roleDb = new Repository<MemberRoleEntity>(Uow);
            var role = roleDb.FindBy(o => o.ID == this.CurrentUser.RoleID).First();

            int quantity = 0;
            decimal total = 0m;
            foreach (var cart in list)
            {
                total += cart.Product.Price * cart.Quantity;
                quantity += cart.Quantity;
            }
            var settingDb = new Repository<SystemConfigEntity>(Uow);
            var price = settingDb.FindBy(o => o.Name == SystemSettingConstants.Price).Single();
            var totalQuantity = total / decimal.Parse(price.ConfigValue);
            totalQuantity = decimal.Round(totalQuantity, 6);

            decimal totalAmount = 0m;
            order.Description = Common.GetTotalAmountDescription1(role.Price, role.RoleRiseDescription, totalQuantity, role.ID, this.CurrentUser.CurrentRoleQuantity, out totalAmount);
            order.Total = totalAmount;

            if (totalAmount < role.OneTimeAmount)
            {
                order.Status = (int)OrderStatus.LessAmount;
            }
            order.Quantity = totalQuantity;

            orderDb.Update(order);
            Uow.Commit();


        }

        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveOrderDetails(Guid ID)
        {
            var db = new Repository<OrderDetailsEntity>(Uow);
            var entity = db.FindBy(o => o.ID == ID).Single();

            var orderDb = new Repository<OrderEntity>(Uow);
            var order = orderDb.FindBy(o => o.ID == entity.OrderID).First();

            //是否需要登记唯一识别码
            if (db.FindBy(o => o.OrderID == entity.OrderID && o.ID != ID).ToList().Any(o => o.Product.HasIdentityCode == true))
            {
                order.Status = (int)OrderStatus.CodeNotFull;
            }
            else
            {
                order.Status = (int)OrderStatus.NoCode;
            }

            db.Remove(entity);

            orderDb.Update(order);

            Uow.Commit();

            UpdateOrder(order.ID);
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="orderId"></param>
        public void RemoveOrder(Guid orderId)
        {
            var detailsDb = new Repository<OrderDetailsEntity>(Uow);
            var list = detailsDb.FindBy(o => o.OrderID == orderId).ToList();
            foreach (var details in list)
            {
                detailsDb.Remove(details);
            }
            var db = new Repository<OrderEntity>(Uow);
            var order = db.FindBy(o => o.ID == orderId).First();
            db.Remove(order);
            Uow.Commit();
        }

        /// <summary>
        /// 保存订单
        /// </summary>
        public void SaveOrder(decimal amount)
        {
            var roleDb = new Repository<MemberRoleEntity>(Uow);
            var role = roleDb.FindBy(o => o.ID == this.CurrentUser.RoleID).First();
            if (amount < role.OneTimeAmount)
            {
                throw new Exception(string.Format("{0}一次进货不能少于{1}元", role.RoleName, role.OneTimeAmount));
            }
        }

        /// <summary>
        /// 获取唯一识别码
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <returns></returns>
        public IdentityCodeSummary NewIdentityCode(Guid orderDetailsId, int page, int rows)
        {

            var result = new IdentityCodeSummary();
            var db = new Repository<IdentityCodeEntity>(Uow);

            Expression<Func<IdentityCodeEntity, bool>> whereExp = o => o.OrderDetailsID == orderDetailsId;

            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

            var list = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * rows).Take(rows).ToList();


            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<OrderDetailsEntity, OrderDetails>();
            Mapper.CreateMap<IdentityCodeEntity, IdentityCode>();

            result.IdentityCodes = Mapper.Map<IEnumerable<IdentityCode>>(list);
            CalculateRowNo(result, result.IdentityCodes, page, rows, totalCount);

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var od = odDb.FindBy(o => o.ID == orderDetailsId).First();

            result.ProductName = od.Product.Name;
            result.Quantity = od.Quantity;
            result.IsDeliverly = od.Order.IsDeliverly;

            return result;
        }

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <returns></returns>
        public void AddNewIdentityCode(Guid orderDetailsId, long codeFrom, long codeTo)
        {
            if (codeFrom == 0 && codeTo == 0)
            {
                throw new Exception("请输入唯一识别码");
            }

            IdentityCode model = new IdentityCode();
            model.OrderDetailsID = orderDetailsId;
            if (codeFrom == 0 || codeTo == 0)
            {
                long code = codeFrom;
                if (code == 0)
                {
                    code = codeTo;
                }

                model.Code = code;

                CodeSingle(model);
            }
            else
            {
                model.CodeFrom = codeFrom;
                model.CodeTo = codeTo;
                CodeRange(model);
            }

        }

        /// <summary>
        /// 单个添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        public void CodeSingle(IdentityCode model)
        {
            if (model.Code == 0)
            {
                throw new Exception("唯一识别码不能为空");
            }
            var db = new Repository<IdentityCodeEntity>(Uow);

            var entity = new IdentityCodeEntity();

            var currentId = this.CurrentUser.ID;
            var now = DateTime.Now;

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var odEntity = odDb.FindBy(o => o.ID == model.OrderDetailsID).First();

            var currentCount = db.FindBy(o => o.OrderDetailsID == model.OrderDetailsID).Count();
            if (currentCount >= odEntity.Quantity)
            {
                throw new Exception("该产品唯一识别码已经添加完毕，不能再添加");
            }

            var productId = odEntity.ProductID;

            //判断当前用户是否有权限提供该唯一识别码
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                if (!db.FindBy(o => model.Code == o.Code && o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID && o.Status == 0 && o.OrderDetails.ProductID == productId).Any())
                {
                    throw new Exception("唯一识别码" + model.Code + "不是有效的唯一识别码，请重新添加");
                }
                //当前用户不能再用这个唯一识别码
                var currEntity = db.FindBy(o => o.Code == model.Code && o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID && o.OrderDetails.ProductID == productId).First();
                currEntity.Status = 1;
                db.Update(currEntity);
            }
            else
            {
                if (db.FindBy(o => o.Code == model.Code).Any())
                {
                    var memberName = db.FindBy(o => o.Code == model.Code).OrderBy(o => o.CreateOn).First().OrderDetails.Order.Member.MemberName;
                    throw new Exception(string.Format("识别码{0}已被{1}使用", model.Code, memberName));
                    //throw new Exception("唯一识别码" + model.Code + "已被使用，不能添加");
                }
            }


            entity.CreateBy = currentId;
            entity.CreateOn = now;
            entity.Code = model.Code;
            entity.ID = Guid.NewGuid();
            entity.Status = 0;
            entity.OrderDetailsID = model.OrderDetailsID;

            db.Add(entity);

            //订单明细唯一识别码已经添加完成
            if (currentCount == odEntity.Quantity - 1)
            {
                odEntity.Status = (int)OrderDetailsStatus.CodeFull;
                odEntity.UpdateBy = currentId;
                odEntity.UpdateOn = now;
                odDb.Update(odEntity);

                //订单唯一识别码已经添加完成
                var count = odDb.FindBy(o => o.OrderID == odEntity.OrderID && o.Status == (int)OrderDetailsStatus.CodeNotFull).Count();
                if (count == 0)
                {
                    var orderDb = new Repository<OrderEntity>(Uow);
                    var order = orderDb.FindBy(o => o.ID == odEntity.OrderID).First();
                    order.Status = (int)OrderStatus.CodeFull;
                    order.UpdateBy = currentId;
                    order.UpdateOn = now;
                    orderDb.Update(order);
                }

            }

            Uow.Commit();

        }

        /// <summary>
        /// 按范围添加唯一识别码
        /// </summary>
        /// <param name="model"></param>
        public void CodeRange(IdentityCode model)
        {
            if (model.CodeFrom == 0)
            {
                throw new Exception("起始唯一识别码不能为空");
            }
            if (model.CodeTo == 0)
            {
                throw new Exception("结束唯一识别码不能为空");
            }
            if (model.CodeTo < model.CodeFrom)
            {
                throw new Exception("结束唯一识别码不能小于起始唯一识别码");
            }
            var db = new Repository<IdentityCodeEntity>(Uow);

            //需要插入的唯一识别码
            var codes = new List<long>();
            for (long i = model.CodeFrom; i <= model.CodeTo; i++)
            {
                codes.Add(i);
            }

            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var orderDetails = odDb.FindBy(o => o.ID == model.OrderDetailsID).First();

            var currentCount = db.FindBy(o => o.OrderDetailsID == model.OrderDetailsID).Count();
            if (currentCount >= orderDetails.Quantity)
            {
                throw new Exception("该订单识别码已经添加完毕，不能再添加");
            }

            var orderQuantity = orderDetails.Quantity;
            var codeQuantity = codes.Count() + currentCount;
            if (orderQuantity < codeQuantity)
            {
                throw new Exception(string.Format("识别码数量{0}超过订货数量{1}，请重新输入", codeQuantity, orderQuantity));
            }

            var productId = orderDetails.ProductID;

            //判断当前用户是否有权限提供该唯一识别码
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {

                //允许使用的唯一识别码
                //var orderDb = new Repository<OrderEntity>(uow);
                //var orders = orderDb.FindBy(o => o.MemberID == this.CurrentUser.MemberID).ToList().Select(o => o.ID);
                //var odIds = odDb.FindBy(o => o.ProductID == productId && orders.Contains(o.OrderID)).ToList().Select(o => o.ID);
                //var mycodes = db.FindBy(o => odIds.Contains(o.OrderDetailsID.Value) && o.Status == 0).ToList();
                var mycodes = db.FindBy(o => o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID && o.Status == 0 && o.OrderDetails.ProductID == productId).ToList();
                var allowedCodes = mycodes.Select(o => o.Code).ToList();

                //不允许使用的唯一识别码
                var unallowedCodes = codes.Where(o => !allowedCodes.Contains(o)).ToList();

                if (unallowedCodes.Any())
                {
                    var distinctCodes = unallowedCodes.Distinct();
                    StringBuilder sb = new StringBuilder();
                    foreach (var code in distinctCodes)
                    {

                        var memberName = db.FindBy(o => o.Code == code).OrderBy(o => o.CreateOn).First().OrderDetails.Order.Member.MemberName;
                        sb.Append(string.Format("{0}已经被{1}使用, ", code, memberName));
                    }

                    throw new Exception(sb.ToString());
                }
            }
            else
            {
                //不允许使用的识别码
                var unallowedCodes = db.FindBy(o => codes.Contains(o.Code) && o.OrderDetails.ProductID == productId).Select(o => o.Code).ToList();
                if (unallowedCodes.Any())
                {
                    var distinctCodes = unallowedCodes.Distinct();
                    StringBuilder sb = new StringBuilder();
                    foreach (var code in distinctCodes)
                    {

                        var memberName = db.FindBy(o => o.Code == code).OrderBy(o => o.CreateOn).First().OrderDetails.Order.Member.MemberName;
                        sb.Append(string.Format("{0}已经被{1}使用, ", code, memberName));
                    }

                    throw new Exception(sb.ToString());
                }
            }

            int status = (int)OrderDetailsStatus.CodeNotFull;

            //订单唯一识别码已经添加完成
            if (currentCount == orderDetails.Quantity - codes.Count())
            {
                status = (int)OrderDetailsStatus.CodeFull;
            }

            //插入数据
            using (SimpleWebUnitOfWork database = new SimpleWebUnitOfWork())
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@codeFrom", model.CodeFrom));
                parameters.Add(new SqlParameter("@codeTo", model.CodeTo));
                parameters.Add(new SqlParameter("@createBy", this.CurrentUser.ID));
                parameters.Add(new SqlParameter("@orderDetailsId", model.OrderDetailsID));
                parameters.Add(new SqlParameter("@status", status));
                database.Database.ExecuteSqlCommand("EXEC [P_BulkAddCodes] @codeFrom, @codeTo, @createBy, @orderDetailsId, @status", parameters.ToArray());
            }

            Uow.Commit();
        }

        /// <summary>
        /// 删除所有识别码
        /// </summary>
        /// <param name="orderDetailsId"></param>
        public void RemoveAllCode(Guid orderDetailsId)
        {
            //删除指定订单明细的所有识别码
            using (SimpleWebUnitOfWork database = new SimpleWebUnitOfWork())
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@orderDetailsId", orderDetailsId));
                database.Database.ExecuteSqlCommand("EXEC [P_BulkRemoveAllCodes] @orderDetailsId", parameters.ToArray());
            }
        }

        /// <summary>
        /// 获取我的库存
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public MemberProductSummary MyStock(int page, int rows)
        {
            return MemberStock(this.CurrentUser.MemberID, page, rows);

        }

        /// <summary>
        /// 获取我的库存
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public MemberProductSummary MemberStock(Guid memberId, int page, int rows)
        {
            var result = new MemberProductSummary();

            var db = new Repository<MemberProductEntity>(Uow);

            Expression<Func<MemberProductEntity, bool>> whereExp = o => o.MemberID == memberId;

            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / rows);

            var memberProducts = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * rows).Take(rows).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<MemberProductEntity, MemberProduct>();

            result.MemberProducts = Mapper.Map<IEnumerable<MemberProduct>>(memberProducts);
            CalculateRowNo(result, result.MemberProducts, page, rows, totalCount);

            return result;
        }

        /// <summary>
        /// 保存库存
        /// </summary>
        /// <param name="memberProductId"></param>
        /// <param name="quantity"></param>
        public void SaveStock(Guid memberProductId, int quantity)
        {
            var db = new Repository<MemberProductEntity>(Uow);
            var entity = db.FindBy(o => o.ID == memberProductId).First();
            entity.Quantity = quantity;
            db.Update(entity);
            Uow.Commit();
        }

        /// <summary>
        /// 获取产品识别码
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IdentityCodeSummary GetCodes(int productId, int page, int size)
        {
            var result = new IdentityCodeSummary();
            var db = new Repository<IdentityCodeEntity>(Uow);
            Expression<Func<IdentityCodeEntity, bool>> whereExp = o => o.ID != Guid.Empty && o.Status == (int)CodeStatus.Available && o.OrderDetails.ProductID == productId && o.OrderDetails.Order.MemberID == this.CurrentUser.MemberID;
            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
            var items = db.Find(whereExp, o => o.CreateOn).OrderBy(o => o.Code).Skip((page - 1) * size).Take(size).ToList();
            Mapper.CreateMap<ProductEntity, Product>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<OrderEntity, Order>();
            Mapper.CreateMap<OrderDetailsEntity, OrderDetails>();
            Mapper.CreateMap<IdentityCodeEntity, IdentityCode>();
            result.IdentityCodes = Mapper.Map<IEnumerable<IdentityCode>>(items);
            CalculateRowNo(result, result.IdentityCodes, page, size, totalCount);
            return result;
        }

        /// <summary>
        /// 获取订单详细信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public string GetOrderDetails(Guid orderId)
        {
            var result = new StringBuilder();

            var db = new Repository<OrderDetailsEntity>(Uow);
            Expression<Func<OrderDetailsEntity, bool>> whereExp = o => o.OrderID == orderId;
            var items = db.Find(whereExp, o => o.CreateOn).ToList();
            foreach (var item in items)
            {
                result.Append(item.Product.Name);
                result.Append(item.Quantity);
                result.Append("件");
                result.Append(",");
            }
            return result.ToString().Substring(0, result.ToString().Length - 1);
        }

        /// <summary>
        /// 获取快递信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="status"></param>
        /// <param name="mobileOrName"></param>
        /// <returns></returns>
        public ExpressSummary GetExpress(int page, int size, int status, string mobileOrName, bool recieve)
        {
            status = status == -1 ? -1 : status;
            var result = new ExpressSummary();
            var db = new Repository<ExpressEntity>(Uow);
            Expression<Func<ExpressEntity, bool>> whereExp = o => o.ID != Guid.Empty;
            if (status != -1)
            {
                whereExp = whereExp.And(o => o.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(mobileOrName))
            {
                if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
                {
                    // 普通用户只能查询收件人
                    whereExp = whereExp.And(o => (o.RecipientMobile.Contains(mobileOrName)
                        || o.RecipientName.ToUpper().Contains(mobileOrName.ToUpper()))
                        );
                }
                else
                {
                    if (recieve)
                    {
                        // 管理员查询收件人
                        whereExp = whereExp.And(o => (o.RecipientMobile.Contains(mobileOrName)
                        || o.RecipientName.ToUpper().Contains(mobileOrName.ToUpper()))
                        );
                    }
                    else
                    {
                        // 管理员查询发件人
                        whereExp = whereExp.And(o => (o.Member.Mobile.Contains(mobileOrName)
                            || o.Member.MemberName.ToUpper().Contains(mobileOrName.ToUpper()))
                            );
                    }
                    
                }

            }
            
            if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
            {
                whereExp = whereExp.And(o => o.MemberID == this.CurrentUser.MemberID);
            }
            var totalCount = db.Find(whereExp, o => o.CreateOn).Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
            var items = db.Find(whereExp, o => o.CreateOn).OrderByDescending(o => o.CreateOn).Skip((page - 1) * size).Take(size).ToList();

            Mapper.CreateMap<MemberRoleEntity, MemberRole>();
            Mapper.CreateMap<MemberEntity, Member>();
            Mapper.CreateMap<ExpressEntity, Express>();
            result.ExpressList = Mapper.Map<IEnumerable<Express>>(items);
            CalculateRowNo(result, result.ExpressList, page, size, totalCount);
            return result;
        }

        /// <summary>
        /// 新增修改快递信息
        /// </summary>
        /// <param name="express"></param>
        public void AddUpdateExpress(Express express)
        {
            var db = new Repository<ExpressEntity>(Uow);

            if (string.IsNullOrWhiteSpace(express.RecipientName))
            {
                throw new Exception("收件人姓名必须输入");
            }

            if (string.IsNullOrWhiteSpace(express.RecipientMobile))
            {
                throw new Exception("收件人手机号码必须输入");
            }

            if (string.IsNullOrWhiteSpace(express.RecipientAddress))
            {
                throw new Exception("收件人地址必须输入");
            }

            if (string.IsNullOrWhiteSpace(express.Content))
            {
                throw new Exception("发货内容必须输入");
            }

            // 管理员操作必须填写快递信息
            if (this.CurrentUser.UserRoleID != (int)enmRoles.General)
            {
                if (string.IsNullOrWhiteSpace(express.ExpressName))
                {
                    throw new Exception("快递名称必须输入");
                }

                if (string.IsNullOrWhiteSpace(express.ExpressNo))
                {
                    throw new Exception("快递单号必须输入");
                }
            }

            var currentUserID = this.CurrentUser.ID;
            var currentMemberID = this.CurrentUser.MemberID;
            var now = DateTime.Now;

            // Update express
            if (express.ID != Guid.Empty)
            {
                //更新
                var entity = db.FindBy(o => o.ID == express.ID).First();
                if (this.CurrentUser.UserRoleID == (int)enmRoles.General)
                {
                    entity.Content = express.Content;
                    entity.UpdateBy = currentUserID;
                    entity.UpdateOn = now;
                    entity.RecipientAddress = express.RecipientAddress;
                    entity.RecipientMobile = express.RecipientMobile;
                    entity.RecipientName = express.RecipientName;
                }
                else
                {

                    entity.Content = express.Content;
                    entity.UpdateBy = currentUserID;
                    entity.UpdateOn = now;
                    entity.RecipientAddress = express.RecipientAddress;
                    entity.RecipientMobile = express.RecipientMobile;
                    entity.RecipientName = express.RecipientName;
                    entity.ExpressName = express.ExpressName;
                    entity.ExpressNo = express.ExpressNo;
                    entity.Status = express.Status;
                    if (express.ExpressDate != null)
                    {
                        entity.ExpressDate = express.ExpressDate;
                    }
                    entity.Status = express.Status;
                    if (entity.Status == 0)
                    {
                        entity.ExpressDate = null;
                    }
                }
                db.Update(entity);

            }
            else
            {
                //新加
                var entity = new ExpressEntity();
                entity.ID = Guid.NewGuid();
                entity.CreateBy = currentUserID;
                entity.CreateOn = now;
                entity.Content = express.Content;
                entity.MemberID = currentMemberID;
                entity.RecipientAddress = express.RecipientAddress;
                entity.RecipientMobile = express.RecipientMobile;
                entity.RecipientName = express.RecipientName;
                entity.Status = 0; // 未发货
                db.Add(entity);

            }

            Uow.Commit();
        }

        /// <summary>
        /// 删除发货记录
        /// </summary>
        /// <param name="id"></param>
        public void RemoveExpress(Guid id)
        {
            var db = new Repository<ExpressEntity>(Uow);

            var entity = db.FindBy(o => o.ID == id).First();
            db.Remove(entity);

            Uow.Commit();
        }

        public void BulkAddCode(Guid orderDetailsId, IEnumerable<long> codeList)
        {
            var db = new Repository<IdentityCodeEntity>(Uow);
            var odDb = new Repository<OrderDetailsEntity>(Uow);
            var orderDetails = odDb.FindBy(o => o.ID == orderDetailsId).First();

            var currentCount = db.FindBy(o => o.OrderDetailsID == orderDetailsId).Count();
            if (currentCount >= orderDetails.Quantity)
            {
                throw new Exception("该订单识别码已经添加完毕，不能再添加");
            }

            var orderQuantity = orderDetails.Quantity;
            var codeQuantity = codeList.Count() + currentCount;
            if (orderQuantity < codeQuantity)
            {
                throw new Exception(string.Format("识别码数量{0}超过订货数量{1}，请重新输入", codeQuantity, orderQuantity));
            }

            IdentityCode model = new IdentityCode();
            model.OrderDetailsID = orderDetailsId;
            foreach (var code in codeList)
            {
                model.Code = code;
                CodeSingle(model);
            }
        }

        ///// <summary>
        ///// 获取编号
        ///// </summary>
        ///// <param name="page"></param>
        ///// <param name="size"></param>
        ///// <param name="mobileOrName"></param>
        ///// <param name="code"></param>
        ///// <param name="productId"></param>
        ///// <returns></returns>
        //public UpdateCodeSummary GetUpdateCodes(int page, int size, string mobileOrName = "", long code = -1, int productId = -1)
        //{
        //    var result = new UpdateCodeSummary();
        //    var db = new Repository<IdentityCodeEntity>(uow);
        //    Expression<Func<IdentityCodeEntity, bool>> whereExp = o => o.ID != Guid.Empty;
        //    if (!string.IsNullOrWhiteSpace(mobileOrName))
        //    {
        //        whereExp = whereExp.And(o => (o.OrderDetails.Order.Member.Mobile.Contains(mobileOrName)
        //        || o.OrderDetails.Order.Member.MemberName.ToUpper().Contains(mobileOrName.ToUpper()))
        //        );
        //    }
        //    if (code != -1)
        //    {
        //        whereExp = whereExp.And(co => co.Code == code);
        //    }
        //    if (productId != -1)
        //    {
        //        whereExp = whereExp.And(o => o.OrderDetails.ProductID == productId);
        //    }
        //    var list = db.Find(whereExp, o => o.CreateOn).Select(o => 
        //        new {
        //            Code = o.Code,
        //            ProductID = o.OrderDetails.ProductID,
        //            ProductName = o.OrderDetails.Product.Name
        //        }).Distinct();
        //    var totalCount = list.Count();
        //    var totalPages = (int)Math.Ceiling((decimal)totalCount / size);
        //    var items = list.OrderByDescending(o => o.Code).Skip((page - 1) * size).Take(size).ToList();
        //    var updateCodeList = new List<UpdateCode>();
        //    var updateCode = new UpdateCode();
        //    foreach(var item in items)
        //    {
        //        updateCode = new UpdateCode();
        //        updateCode.Code = item.Code;
        //        updateCode.ProductID = item.ProductID;
        //        updateCode.ProductName = item.ProductName;
        //        updateCodeList.Add(updateCode);
        //    }
        //    result.UpdateCodeList = updateCodeList;
        //    CalculateRowNo(result, result.UpdateCodeList, page, size, totalCount);
        //    return result;
        //}

        /// <summary>
        /// 更改编号
        /// </summary>
        /// <param name="code"></param>
        /// <param name="productId"></param>
        public void UpdateCode(long newCode, long oldCode, int productId)
        {
            var db = new Repository<IdentityCodeEntity>(Uow);
            // check whether the newCode is used
            if (db.FindBy(o => o.Code == newCode && o.OrderDetails.ProductID == productId).Any())
            {
                var memberName = db.FindBy(o => o.Code == newCode).OrderBy(o => o.CreateOn).First().OrderDetails.Order.Member.MemberName;
                throw new Exception(string.Format("识别码{0}已被{1}使用", newCode, memberName));
            }
            var list = db.FindBy(o => o.Code == oldCode && o.OrderDetails.ProductID == productId).ToList();
            foreach (var item in list)
            {
                item.Code = newCode;
                db.Update(item);
            }
            Uow.Commit();
        }

        /// <summary>
        /// 获取总代订单
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public GeneralOrderSummary GeneralOrder(int page, int size, DateTime? dateFrom, DateTime? dateTo)
        {
            var result = new GeneralOrderSummary();
            var orderList = new List<GeneralOrderModel>();
            Repository<OrderEntity> orderRepo = new Repository<OrderEntity>(Uow);
            Repository<MemberEntity> memberRepo = new Repository<MemberEntity>(Uow);
            var orderListForGeneral1 = orderRepo.FindBy(o => o.GeneralAgent1ID == this.CurrentUser.MemberID && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            var orderListForGeneral2 = orderRepo.FindBy(o => o.GeneralAgent2ID == this.CurrentUser.MemberID && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            var orderListForGeneral3 = orderRepo.FindBy(o => o.GeneralAgent3ID == this.CurrentUser.MemberID && o.IsDeliverly && o.SendMemberID == Guid.Empty);
            if (dateFrom.HasValue)
            {
                DateTime dtFrom = new DateTime(dateFrom.Value.Year, dateFrom.Value.Month, dateFrom.Value.Day, 0, 0, 0);
                orderListForGeneral1 = orderListForGeneral1.Where(o => o.CreateOn >= dtFrom);
                orderListForGeneral2 = orderListForGeneral2.Where(o => o.CreateOn >= dtFrom);
                orderListForGeneral3 = orderListForGeneral3.Where(o => o.CreateOn >= dtFrom);
            }
            if (dateTo.HasValue)
            {
                DateTime dtTo = new DateTime(dateTo.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
                orderListForGeneral1 = orderListForGeneral1.Where(o => o.CreateOn <= dtTo);
                orderListForGeneral2 = orderListForGeneral2.Where(o => o.CreateOn <= dtTo);
                orderListForGeneral3 = orderListForGeneral3.Where(o => o.CreateOn <= dtTo);
            }
            if (orderListForGeneral1 != null && orderListForGeneral1.Any())
            {
                var general1List = orderListForGeneral1.GroupBy(o => o.MemberID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();



                if (memberRepo.FindBy(o => general1List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general1List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        orderList.Add(new GeneralOrderModel()
                        {
                            Level = 1,
                            MemberName = member.MemberName,
                            Quantity = general1List.Where(o => o.key == member.ID).First().sum
                        });
                    }
                }
            }

            if (orderListForGeneral2 != null && orderListForGeneral2.Any())
            {
                var general2List = orderListForGeneral2.GroupBy(o => o.MemberID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();



                if (memberRepo.FindBy(o => general2List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general2List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        orderList.Add(new GeneralOrderModel()
                        {
                            Level = 2,
                            MemberName = member.MemberName,
                            Quantity = general2List.Where(o => o.key == member.ID).First().sum
                        });
                    }
                }
            }

            if (orderListForGeneral3 != null && orderListForGeneral3.Any())
            {
                var general3List = orderListForGeneral3.GroupBy(o => o.MemberID)
                    .Select(g => new
                    {
                        key = g.Key,
                        sum = g.Sum(a => a.Quantity)
                    }).ToList();



                if (memberRepo.FindBy(o => general3List.Any(g => g.key == o.ID)).Any())
                {
                    var memberList = memberRepo.FindBy(o => general3List.Any(g => g.key == o.ID)).ToList();
                    foreach (var member in memberList)
                    {
                        orderList.Add(new GeneralOrderModel()
                        {
                            Level = 3,
                            MemberName = member.MemberName,
                            Quantity = general3List.Where(o => o.key == member.ID).First().sum
                        });
                    }
                }
            }

            var totalCount = orderList.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / size);

            result.GeneralOrderList = orderList;
            CalculateRowNo(result, result.GeneralOrderList, page, size, totalCount);

            return result;

        }

        public void Register(RegisterModel model)
        {
            var member = new MemberEntity();
            member.ID = model.Member.MemberId;
            member.Mobile = model.Member.Mobile;
            member.MemberName = model.Member.MemberName;
            member.CreateBy = model.User.Id;
            member.CreateOn = DateTime.Now;
            member.ParentMemberID = model.Member.ParentId;
            member.MemberName = model.Member.MemberName;
            member.Address = model.Member.Address;
            member.IdentityNo = model.Member.IdentityNo;
            member.Address = GetAddress(model);
            member.ProvinceAvailable = model.Member.ProvinceAvailable;
            member.GeneralAvailable = model.Member.GeneralAvailable;
            member.TotalAmount = model.Member.TotalAmount;
            member.CurrentRoleAmount = model.Member.CurrentRoleAmount;
            member.RoleID = model.Member.RoleId;
            MemDb.Add(member);
        }
        private string GetAddress(RegisterModel model)
        {
            var address = model.MemberAddress;
            return address.Province.ProvinceName +
                address.City.CityName +
                address.District.DistrictName +
                address.Street.StreetName +
                address.Description;
        }
    }
}
