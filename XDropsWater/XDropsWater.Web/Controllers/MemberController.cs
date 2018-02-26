using XDropsWater.Bll.Interface;
using XDropsWater.Model;
using XDropsWater.Web.Utility;
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace XDropsWater.Web.Controllers
{
    public class MemberController : BaseController
    {
        readonly IMemberService service;
        public MemberController(IMemberService service)
        {
            this.service = service;
        }
        public ActionResult Check()
        {
            return View();
        }
        //
        // GET: /Member/

        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// 高号管理，由公司直接给高级别的人员成为高号
        /// </summary>
        /// <returns></returns>
        public ActionResult HighNumberManage()
        {
            return View();
        }

        public ActionResult RecommendMemberLocation()
        {
            return View();
        }

        public ActionResult Test(int page = 1, int rows = 10, string search = "")
        {
            //List<MemberModel> list = service.GetMember(search);
            //return Request.IsAjaxRequest()
            //    ? (ActionResult)PartialView("ProductList", list.ToPagedList(page, rows))
            //    : View(list.ToPagedList(page, rows));
            return View();
        }

        public ActionResult MemberManage(int page = 1, int rows = 10, string search = "")
        {
            List<MemberModel> list = service.GetMember(search);
            //List<MemberModel> list = null;
            AddMemberModel addMemberModel = new AddMemberModel();
            addMemberModel.RoleList = service.GetMemberRole();
            list.ForEach(item => item.MemberRoleName = addMemberModel.RoleList.First(o => o.Id == item.MemberRoleID).Name);
            var validRoleList = new List<ValidRoleModel>();
            validRoleList.Add(new ValidRoleModel() { Id = 0, Name = "无效" });
            validRoleList.Add(new ValidRoleModel() { Id = 1, Name = "有效" });
            addMemberModel.ProvinceAvailableList = validRoleList;
            addMemberModel.GeneralAvailableList = validRoleList;
            var allModels = new Tuple<IPagedList<MemberModel>, AddMemberModel>
                (list.ToPagedList(page, rows), addMemberModel)
            { };

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("ProductList", allModels.Item1)
                : View(allModels);
        }

        [HttpGet]
        public ActionResult MemberManage1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MemberManage1(int page = 1, int rows = 10, string mobileOrName = "")
        {
            var list = service.GetMember1(page, rows, mobileOrName);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int GetMembersCount()
        {
            return service.GetMembersCount();
        }

        public ActionResult MemberOrderSearch(string mobileOrName, DateTime? dateFrom, DateTime? dateTo, int? roleId, int page = 1, int rows = 10)
        {

            if (!roleId.HasValue)
                roleId = -1;
            List<MemberModel> list = service.MemberOrderSearch(mobileOrName, dateFrom, dateTo, roleId.Value);

            var roleList = service.GetMemberRole();
            list.ForEach(item => item.MemberRoleName = roleList.First(o => o.Id == item.MemberRoleID).Name);

            MemberModel model = new MemberModel();

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("MemberOrderSearchList", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        public ActionResult MemberOrderManage(string mobileOrName, bool? isDelivery, int page = 1, int rows = 10)
        {
            //var service = this.GetService<IMemberService>();
            //List<MemberModel> list = service.GetMember(search);
            //AddMemberModel addMemberModel = new AddMemberModel();
            //addMemberModel.RoleList = service.GetMemberRole();
            //list.ForEach(item => item.MemberRoleName = addMemberModel.RoleList.First(o => o.Id == item.MemberRoleID).Name);
            //var allModels = new Tuple<IPagedList<MemberModel>, AddMemberModel>
            //    (list.ToPagedList(page, rows), addMemberModel) { };

            //return Request.IsAjaxRequest()
            //    ? (ActionResult)PartialView("ProductList", allModels.Item1)
            //    : View(allModels);


            List<MemberOrderModel> list = service.GetMemberOrder(mobileOrName, isDelivery);

            var roleList = service.GetMemberRole();
            list.ForEach(item => item.MemberRoleName = roleList.First(o => o.Id == item.MemberRoleID).Name);

            MemberOrderModel model = new MemberOrderModel();

            var allModels = new Tuple<IPagedList<MemberOrderModel>, MemberOrderModel>
                (list.ToPagedList(page, rows), model)
            { };

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("MemberOrderList", allModels.Item1)
                : View(allModels);
        }

        public ActionResult MemberOrderManage1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MemberOrderManage1(string mobileOrName, bool? isDelivery, int page = 1, int rows = 10)
        {
            var orderLevel = enmOrderLevel.Company;
            var user = service.CurrentUser;
            if (user.UserRoleID == (int)enmRoles.General)
            {
                orderLevel = enmOrderLevel.Agency;
            }
            var orderSummary = service.GetMemberOrder1(page, rows, mobileOrName, isDelivery, orderLevel);
            return Json(orderSummary, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取产品唯一辨识码
        /// </summary>
        /// <param name="orderDetailsId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GetCodePages(Guid orderDetailsId, int page = 1, int rows = 10)
        {
            var codePages = service.GetCodePages(page, rows, orderDetailsId);
            return Json(codePages, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取产品识别码
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCodes(int productId, int page = 1, int rows = 10)
        {
            var result = service.GetCodes(productId, page, rows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PersonalOrder(int page = 1, int rows = 10, bool? isDeliverly = null)
        {

            List<MemberOrderModel> list = service.GetPersonalOrder(isDeliverly);

            AddPersonalOrderModel addModel = new AddPersonalOrderModel();
            var allModels = new Tuple<IPagedList<MemberOrderModel>, AddPersonalOrderModel>
               (list.ToPagedList(page, rows), addModel)
            { };


            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PersonalOrderList", allModels.Item1)
                : View(allModels);
        }

        public ActionResult PersonalOrder1()
        {

            return View();
        }

        /// <summary>
        /// get personal orders
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="isDeliverly"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PersonalOrder1(int page = 1, int rows = 10, int isDeliverly = -1)
        {
            bool? bDeliverly = null;
            if (isDeliverly == 0)
                bDeliverly = false;
            else if (isDeliverly == 1)
                bDeliverly = true;
            var orderSummary = service.GetPersonalOrder1(page, rows, bDeliverly);

            return Json(orderSummary, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ChildMemberOrder(string mobileOrName, bool? isDelivery, int page = 1, int rows = 10)
        {
            //Tuple<IPagedList<MemberOrderModel>, MemberOrderModel>

            List<MemberOrderModel> list = service.GetMemberOrder(mobileOrName, isDelivery);
            MemberOrderModel addModel = new MemberOrderModel();

            var allModels = new Tuple<IPagedList<MemberOrderModel>, MemberOrderModel>
               (list.ToPagedList(page, rows), addModel)
            { };

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("ChildMemberOrderList", allModels.Item1)
                : View(allModels);
        }

        /// <summary>
        /// 向我订货的订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ChildMemberOrder1()
        {
            return View();
        }

        /// <summary>
        /// 向我订货的订单
        /// </summary>
        /// <param name="mobileOrName"></param>
        /// <param name="isDelivery"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChildMemberOrder1(string mobileOrName, bool? isDelivery, int page = 1, int rows = 10)
        {
            var orderLevel = enmOrderLevel.Company;
            var user = service.CurrentUser;
            if (user.UserRoleID == (int)enmRoles.General)
            {
                orderLevel = enmOrderLevel.Agency;
            }
            var orderSummary = service.GetMemberOrder1(page, rows, mobileOrName, isDelivery, orderLevel);
            return Json(orderSummary, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 修改代理所在的门店
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateMemberStore()
        {
            return View();
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalInfo()
        {
            var member = service.GetPersonalInfo();

            return View(member);
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalInfo1()
        {
            return View();
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPersonalInfo1()
        {
            var personalInfo = service.GetPersonalInfo1();
            return Json(personalInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 直属下级代理管理
        /// </summary>
        /// <returns></returns>
        public ActionResult DirectChildMember(string mobileOrName, int page = 1, int rows = 10)
        {
            List<MemberModel> list = service.GetDirectChildMember(mobileOrName);

            var roleList = service.GetMemberRole();
            list.ForEach(item => item.MemberRoleName = roleList.First(o => o.Id == item.MemberRoleID).Name);

            AddChildMemberModel model = new AddChildMemberModel();

            var allModels = new Tuple<IPagedList<MemberModel>, AddChildMemberModel>
                (list.ToPagedList(page, rows), model)
            { };

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("ChildMemberList", allModels.Item1)
                : View(allModels);
        }

        /// <summary>
        /// 直属下级代理管理
        /// </summary>
        /// <returns></returns>
        public ActionResult DirectChildMember1()
        {
            return View();
        }

        /// <summary>
        /// 直属下级代理管理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DirectChildMember1(string mobileOrName, int page = 1, int rows = 10)
        {
            var list = service.GetDirectChildMember1(page, rows, mobileOrName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 查部门信息
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartInfo()
        {
            return View();
        }

        public JsonResult Update(MemberModel member)
        {
            var errCode = service.Update(member);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateMemberForCheck(string cardNo, string memberName,
            string bankName, string bankCardNo, string mobile, string address, string recommendMemberCardNo, string parentCardNo)
        {
            MemberModel model = new MemberModel();
            model.IdentityCardNo = cardNo.Trim();
            model.MemberName = memberName.Trim();
            model.BankName = bankName.Trim();
            model.BankCardNo = bankCardNo.Trim();
            model.Mobile = mobile.Trim();
            model.Address = address.Trim();
            model.RecommendIdentityCardNo = recommendMemberCardNo.Trim();
            model.ParentIdentityCardNo = parentCardNo.Trim();
            var msg = service.UpdateMemberForCheck(model);
            return Json(msg);
        }
        public JsonResult Add(string cardNo, string memberName)
        {
            var member = new MemberModel();
            member.IdentityCardNo = cardNo;
            member.MemberName = memberName;
            var errCode = service.Add(member);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get(string cardNo, string memberName, int page = 1, int rows = 10)
        {
            int total = 0;
            var list = service.Get(cardNo, memberName, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberForCheck(string userId, string cardNo, int hasChecked, int page = 1, int rows = 10)
        {
            Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
            Guid guid1 = guid;
            Guid.TryParse(userId, out guid1);
            List<MemberModel> list = new List<MemberModel>();
            int total = 0;

            list = service.GetMemberForCheck(guid1, cardNo, hasChecked, ref total, page, rows);
            var json1 = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json1, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取高号数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardNo"></param>
        /// <param name="hasChecked"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetMemberHighNumber(string memberName, string cardNo, int page = 1, int rows = 10)
        {

            int total = 0;

            List<MemberModel> list = service.GetMemberForHighNumber(memberName, cardNo, ref total, page, rows);
            var json1 = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMember(int page = 1, int rows = 10, string search = "")
        {

            //    var service = this.GetService<IMemberService>();
            //    List<MemberModel> list = service.GetMember(search);
            //    var memberList = new StaticPagedList<MemberModel>(list, page, rows, list.Count());
            //    return Json(new { Members = list, pager = memberList.GetMetaData() }, JsonRequestBehavior.AllowGet);
            //List<MemberModel> list = service.GetMember(search);
            List<MemberModel> list = new List<MemberModel>();
            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("ProductList", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        public JsonResult GetPersonalInfo()
        {

            var member = service.GetPersonalInfo();

            return Json(member, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetDirectChildMember(string mobile, string memberName, int page = 1, int rows = 10)
        {

            int total = 0;

            List<MemberModel> list = service.GetDirectChildMember(mobile);
            var json1 = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberOrder(string mobile, bool? isDelivery, int page = 1, int rows = 10)
        {

            int total = 0;

            List<MemberOrderModel> list = service.GetMemberOrder(mobile, isDelivery);
            var json1 = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json1, JsonRequestBehavior.AllowGet);
        }



        public JsonResult CheckedByOperator(Guid memberId)
        {
            var errCode = service.CheckedByOperator(memberId);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }

        public string AddHighNumber(string cardNo, string memberName,
            string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId, int agencyLevelId)
        {
            return service.AddHeighNumber(cardNo, memberName, bankName, bankCardNo, parentCardNo, recommendCardNo, cardLevelId, agencyLevelId);
        }

        [HttpPost]
        public ActionResult AddMember(Member model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelState.GetErrors());
            }
            service.AddMember(model);
            return new EmptyResult();
        }

        /// <summary>
        /// 后台直接添加代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMember1(Member model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelState.GetErrors());
            }
            service.AddMember1(model);
            return new EmptyResult();
        }

        public string AddMemberOrder(string mobile, string quantity)
        {
            return service.AddMemberOrder(mobile, quantity);
        }

        [HttpPost]
        public ActionResult AddPersonalOrder(AddPersonalOrderModel model)
        {
            if (ModelState.IsValid)
            {
                service.AddPersonalOrder(model);
                return new EmptyResult();
            }
            else
            {
                throw new Exception(ModelState.GetErrors());
            }
        }

        [HttpPost]
        public ActionResult UpdatePersonalOrder(AddPersonalOrderModel model)
        {
            if (ModelState.IsValid)
            {
                service.UpdatePersonalOrder(model);
                return new EmptyResult();
            }
            else
            {
                throw new Exception(ModelState.GetErrors());
            }
        }

        public string UpdateMemberOrder(Guid memberId, string quantity)
        {
            return service.UpdateMemberOrder(memberId, quantity);
        }

        [HttpPost]
        public ActionResult PersonalInfo(MemberModel model)
        {
            service.SavePersonalInfo(model);
            return new EmptyResult();
        }

        /// <summary>
        /// 保存个人信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBaseInfo(BaseInfo model)
        {
            service.SaveBaseInfo(model);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult RemoveMemberOrder(Guid orderId)
        {
            service.RemoveMemberOrder(orderId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult SendMemberOrder(Guid orderId, string expressContent)
        {
            service.SendMemberOrder(orderId, expressContent);
            return new EmptyResult();
        }

        public string UpdateHighNumber(Guid memberId, string cardNo, string memberName,
            string bankName, string bankCardNo, string parentCardNo, string recommendCardNo, int cardLevelId)
        {
            return service.UpdateHeighNumber(memberId, cardNo, memberName, bankName, bankCardNo, parentCardNo, recommendCardNo, cardLevelId);

        }

        [HttpPost]
        public ActionResult UpdateMember(AddMemberModel model)
        {
            service.UpdateChildMember(model);
            return new EmptyResult();

        }

        [HttpPost]
        public ActionResult UpdateCurrentRoleStock(AddMemberModel model)
        {
            service.UpdateCurrentRoleStock(model);
            return new EmptyResult();

        }

        /// <summary>
        /// 后台管理员修改代理
        /// </summary>
        /// <param name="model">要修改的代理对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateMemberForAdmin(Member model)
        {
            service.UpdateChildMemberForAdmin(model);
            return new EmptyResult();

        }

        /// <summary>
        /// 后台管理员修改代理
        /// </summary>
        /// <param name="model">要修改的代理对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateMemberForAdmin1(Member model)
        {
            service.UpdateChildMemberForAdmin1(model);
            return new EmptyResult();

        }

        [HttpPost]
        public ActionResult UpdateOrderExpress(Guid orderId, string expressContent)
        {
            service.UpdateOrderExpress(orderId, expressContent);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult RemoveCode(Guid Id)
        {
            service.RemoveCode(Id);
            return new EmptyResult();
        }

        /// <summary>
        /// 删除所有识别码
        /// </summary>
        /// <param name="orderDetailsId">订单明细ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveAllCode(Guid orderDetailsId)
        {
            service.RemoveAllCode(orderDetailsId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult RemoveMember(Guid id)
        {
            service.RemoveMember1(id);
            return new EmptyResult();
        }

        /// <summary>
        /// remove member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveMember1(Guid memberId)
        {
            service.RemoveMember1(memberId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddDirectChildMember(AddChildMemberModel model)
        {
            service.AddDirectChildMember(model);
            return new EmptyResult();

        }

        /// <summary>
        /// add a member
        /// </summary>
        /// <param name="model">member object</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDirectChildMember1(AddUpdateMember model)
        {
            service.AddDirectChildMember1(model);
            return new EmptyResult();

        }

        /// <summary>
        /// 获取当前角色下的产品信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRoleProducts()
        {
            var productSummary = service.GetProductMemberRole();
            return Json(productSummary, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ResetPassword(Guid memberId)
        {
            service.ResetPassword(memberId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddCode(IdentityCode model)
        {
            if (model.CodeFrom != 0 && model.CodeTo != 0)
            {
                service.AddCodeRange(model);
            }
            else
            {
                service.AddCode(model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="Products"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrder(IEnumerable<ProductMemberRole> products)
        {
            service.AddOrder(products);
            return new EmptyResult();
        }

        public ActionResult ShoppingCart()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShoppingCart(int page = 1, int rows = 10)
        {
            var shoppingCart = service.ShoppingCart(page, rows);
            return Json(shoppingCart, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 推荐的人能放在哪些代理的后面
        /// </summary>
        /// <param name="recommendCardNo"></param>
        /// <returns></returns>
        public JsonResult GetRecommendMemberLocation(string recommendCardNo, int page = 1, int rows = 10)
        {
            int total = 0;
            List<MemberModel> list = new List<MemberModel>();
            list = service.GetRecommendMemberLocation(recommendCardNo, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// 获取代理信息，用来修改专卖店账号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="memberName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetMemberForUpdateStore(string cardNo, string name, int page = 1, int rows = 10)
        {
            int total = 0;
            var list = service.GetMemberForUpdateStore(cardNo, name, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public string UpdateStore(string cardNo, string account)
        {
            return service.UpdateStore(cardNo, account);
        }

        /// <summary>
        /// 推荐的人能放在哪些代理的后面
        /// </summary>
        /// <param name="recommendCardNo"></param>
        /// <returns></returns>
        public JsonResult GetDepartInfo(string cardNo)
        {
            List<MemberModel> list = new List<MemberModel>();
            list = service.GetDepartInfo(cardNo);
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 根据电话号码获取代理对象
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMemberName(string mobile)
        {
            var member = service.GetMemberByMobile(mobile);
            return Json(member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public ActionResult CheckMobile(string mobile)
        {
            try
            {
                MemberModel m = service.Get(mobile);
                JsonResult js = null;
                if (m != null)
                {
                    if (string.IsNullOrWhiteSpace(m.MemberName))
                    { js = Json(new { valid = false }, JsonRequestBehavior.AllowGet); }
                    else
                        js = Json(new { valid = true }, JsonRequestBehavior.AllowGet);
                }

                else
                    js = Json(new { valid = false }, JsonRequestBehavior.AllowGet);
                return js;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            service.ChangePassword(model);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult ChangePassword1(UserPassword model)
        {
            service.ChangePassword1(model);
            return new EmptyResult();
        }

        /// <summary>
        /// 所有下级
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllSubMembers(string mobileOrName, int levelId = -1, int page = 1, int rows = 10)
        {
            List<SubMemberModel> list = service.GetAllSubMembers(mobileOrName, levelId);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("GetAllSubMembersList", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        [HttpGet]
        public ActionResult GetAllSubMembers1()
        {
            return View();
        }

        /// <summary>
        /// 所有下级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllSubMembers1(string mobileOrName, int levelId = -1, int page = 1, int rows = 10)
        {

            var result = service.GetAllSubMembers1(page, rows, mobileOrName, levelId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHighSubMembers(string mobileOrName, int page = 1, int rows = 10)
        {
            List<SubMemberModel> list = service.GetHighSubMembers(mobileOrName);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("GetHighSubMembersList", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        /// <summary>
        /// 级别>=我
        /// </summary>
        /// <returns></returns>
        public ActionResult GetHighSubMembers1()
        {
            return View();
        }

        /// <summary>
        /// 级别>=我
        /// </summary>
        /// <param name="mobileOrName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetHighSubMembers1(string mobileOrName, int page = 1, int rows = 10)
        {
            var result = service.GetHighSubMembers1(page, rows, mobileOrName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BackOfficeManage()
        {
            return View();
        }

        public ActionResult CalculateTotal()
        {
            service.CalculateTotalOrder();
            return new EmptyResult();
        }

        public ActionResult SetValidAgent()
        {
            service.SetAgentValidRole();
            return new EmptyResult();
        }

        public ActionResult UpdateParentChild()
        {
            service.UpdateParentChild();
            return new EmptyResult();
        }

        /// <summary>
        /// 更新董事
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateDirector()
        {
            service.UpdateDirector();
            return new EmptyResult();
        }

        /// <summary>
        /// 清空上下级关系表
        /// </summary>
        /// <returns></returns>
        public ActionResult CleanUpParentChild()
        {
            service.CleanUpParentChild();
            return new EmptyResult();
        }

        /// <summary>
        /// 更新订单上面三个总代
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateOrderGeneral()
        {
            service.UpdateOrderGeneral();
            return new EmptyResult();
        }

        /// <summary>
        /// 更新董事日期
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateDirectorDate()
        {
            service.UpdateDirectorDate();
            return new EmptyResult();
        }

        /// <summary>
        /// 更新获奖的董事
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateAwardDirector()
        {
            service.UpdateAwardDirector();
            return new EmptyResult();
        }

        /// <summary>
        /// 更新订单发货日期
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateOrderSendDate()
        {
            service.UpdateOrderSendDate();
            return new EmptyResult();
        }

        public ActionResult Member503020(string mobileOrName, DateTime? dateFrom, DateTime? dateTo, int page = 1, int rows = 10)
        {
            List<Member503020> list = service.Calculate503020(mobileOrName, dateFrom, dateTo);

            //var roleList = service.GetMemberRole();
            //list.ForEach(item => item.RoleName = roleList.First(o => o.Id == item.RoleId).Name);
            list.ForEach(item => item.Total = item.Count20 * 20 + item.Count30 * 30 + item.Count50 * 50);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("Member503020List", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        public ActionResult Details503020(int detailsType, Guid memberId, DateTime? dateFrom, DateTime? dateTo, string yearMonth = "", int page = 1, int rows = 10)
        {
            List<DetailsModel> list = service.Details503020(detailsType, memberId, dateFrom, dateTo);
            ViewBag.Sum = list.Sum(o => o.Quantity);

            if (detailsType == (int)enmDetailsType.typeCompany)
            {
                ViewBag.MemberCount = service.CalcualteCompanyBonusMemberCount(yearMonth);
            }

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("Details503020List", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        public ActionResult DirectorBonus(string mobileOrName, string yearMonth, int page = 1, int rows = 10)
        {
            if (string.IsNullOrWhiteSpace(yearMonth))
            {
                yearMonth = DateTime.Now.ToString("yyyyMM");
            }

            List<DirectorBonusModel> list = service.CalculateDirectorBonus(mobileOrName, yearMonth);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("DirectorBonusList", list.ToPagedList(page, rows))
                : View(list.ToPagedList(page, rows));
        }

        [HttpPost]
        public ActionResult AddUpdateShoppingCart(ShoppingCart model)
        {
            service.AddUpdateShoppingCart(model);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetProducts()
        {
            var result = service.GetProducts();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveShoppingCart(Guid ID)
        {
            service.RemoveShoppingCart(ID);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddOrder1()
        {
            service.AddOrder1();
            return new EmptyResult();
        }

        public ActionResult UpdateOrder()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OrderDetails(Guid orderId, int page = 1, int rows = 10)
        {
            var result = service.OrderDetails(orderId, page, rows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新订单明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUpdateOrderDetails(OrderDetails model)
        {
            service.AddUpdateOrderDetails(model);
            return new EmptyResult();
        }

        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveOrderDetails(Guid ID)
        {
            service.RemoveOrderDetails(ID);
            return new EmptyResult();
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveOrder(Guid orderId)
        {
            service.RemoveOrder(orderId);
            return new EmptyResult();
        }

        /// <summary>
        /// 登记唯一识别码
        /// </summary>
        /// <returns></returns>
        public ActionResult AddIdentityCode()
        {
            return View();
        }

        /// <summary>
        /// 保存订单
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveOrder(decimal amount)
        {
            service.SaveOrder(amount);
            return new EmptyResult();
        }

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <returns></returns>
        public ActionResult NewIdentityCode()
        {
            return View();
        }

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewIdentityCode(Guid orderDetailsId, int page = 1, int rows = 10)
        {
            var result = service.NewIdentityCode(orderDetailsId, page, rows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加唯一识别码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewIdentityCode(Guid orderDetailsId, long codeFrom, long codeTo)
        {
            service.AddNewIdentityCode(orderDetailsId, codeFrom, codeTo);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult BulkAddCode(Guid orderDetailsId, IEnumerable<long> codeList)
        {
            service.BulkAddCode(orderDetailsId, codeList);
            return new EmptyResult();
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="expressContent"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendMemberOrder1(Guid orderId, string expressContent)
        {
            service.SendMemberOrder1(orderId, expressContent);
            return new EmptyResult();
        }

        /// <summary>
        /// 确认收款
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FinancialConfirm(Guid orderId)
        {
            service.FinancialConfirm(orderId);
            return new EmptyResult();
        }

        /// <summary>
        /// 我的库存
        /// </summary>
        /// <returns></returns>
        public ActionResult MyStock()
        {
            return View();
        }

        /// <summary>
        /// 获取我的库存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MyStock(int page, int rows)
        {
            var result = service.MyStock(page, rows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 代理库存
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberStock()
        {
            return View();
        }

        /// <summary>
        /// 保存库存
        /// </summary>
        /// <param name="memberProductId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveStock(Guid memberProductId, int quantity)
        {

            service.SaveStock(memberProductId, quantity);
            return new EmptyResult();
        }

        /// <summary>
        /// 获取代理库存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MemberStock(Guid memberId, int page, int rows)
        {
            var result = service.MemberStock(memberId, page, rows);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取订单明细信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrderDetails(Guid orderId)
        {
            var result = service.GetOrderDetails(orderId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发货信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Express()
        {
            return View();
        }

        /// <summary>
        /// 获取快递信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="status"></param>
        /// <param name="mobileOrName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetExpress(int page, int size, int status, string mobileOrName)
        {
            var result = service.GetExpress(page, size, status, mobileOrName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加发货记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUpdateExpress(Express model)
        {
            service.AddUpdateExpress(model);
            return new EmptyResult();
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendExpress(Express model)
        {
            model.ExpressDate = DateTime.Now;
            model.Status = 1;
            service.AddUpdateExpress(model);
            return new EmptyResult();
        }


        /// <summary>
        /// 删除发货记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveExpress(Guid id)
        {
            service.RemoveExpress(id);
            return new EmptyResult();
        }

        /// <summary>
        /// 修改识别码
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateCodes()
        {
            return View();
        }

        /// <summary>
        /// 获取修改识别码
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="mobileOrName"></param>
        /// <param name="code"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUpdateCodes(int page, int rows, string mobileOrName = "", long code = -1, int productId = -1)
        {
            //var result = service.GetUpdateCodes(page, rows, mobileOrName, code, productId);
            //return Json(result, JsonRequestBehavior.AllowGet);
            return null;
        }
    }
}
