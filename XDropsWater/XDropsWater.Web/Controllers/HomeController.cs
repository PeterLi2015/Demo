using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Model;
using XDropsWater.Bll;
using XDropsWater.Bll.Interface;

namespace XDropsWater.Web.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        //public ActionResult Index()
        //{
        //    if (this.Session["CurUser"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    ViewBag.UserName = (this.Session["CurUser"] as UserSummary).Name;
        //    ViewBag.Account = (this.Session["CurUser"] as UserSummary).Account;
        //    return View();
        //}

        public ActionResult Index()
        {
            if (this.Session["CurUser"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.UserName = (this.Session["CurUser"] as UserSummary).UserName;
            ViewBag.Account = (this.Session["CurUser"] as UserSummary).Account;
            UserSummary user = Session["CurUser"] as UserSummary;
            var menuGroups = new List<MenuGroup>();
            var service = this.GetService<IMemberService>();
            ViewBag.NewOrderCount = service.GetNewOrderCount();
            ViewBag.MemberCount = service.GetMemberCount();
            ViewBag.NewExpressCount = service.GetNewExpress();
            if (user.UserRoleID == (int)enmRoles.Admin || user.UserRoleID == (int)enmRoles.All)
            {
                MenuGroup menu = new MenuGroup();
                menu.Name = "系统管理";
                menu.MenuItems = new List<MenuItem>() {
                        new MenuItem(){  Icon = "icon-sys",Id="MemberManage", Name="代理管理", Url="/Member/MemberManage1?SystemMenu=Menu"},
                        new MenuItem(){  Icon = "icon-sys",Id="MemberOrderManage", Name="代理订单管理", Url="/Member/MemberOrderManage1?SystemMenu=Menu"},
                        new MenuItem(){  Icon = "icon-sys",Id="MemberOrderSearch", Name="代理订单查询", Url="/Member/MemberOrderSearch?SystemMenu=Menu"},
                        new MenuItem(){  Icon = "icon-sys",Id="Member503020", Name="总代50-30-20分红", Url="/Member/Member503020?SystemMenu=Menu"},
                        new MenuItem(){  Icon = "icon-sys",Id="DirectorBonus", Name="董事加权分红", Url="/Member/DirectorBonus?SystemMenu=Menu"}
                };
                if (user.UserRoleID == (int)enmRoles.All)
                {
                    menu.MenuItems.Add(new MenuItem() { Icon = "icon-sys", Id = "BackOfficeManage", Name = "后台管理", Url = "/Member/BackOfficeManage?SystemMenu=Menu" });
                }
                menuGroups.Add(menu);
            }
            else if (user.UserRoleID == (int)enmRoles.General)
            {
                menuGroups.Add(new MenuGroup()
                {
                    Name = "系统管理",
                    MenuItems = new List<MenuItem>() { 
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="我的订单", Url="/Member/PersonalOrder1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="代理订单", Url="/Member/ChildMemberOrder1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="添加代理", Url="/Member/DirectChildMember1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="所有代理", Url="/Member/GetAllSubMembers1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="代理级别≥我", Url="/Member/GetHighSubMembers1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="我的库存", Url="/Member/MyStock?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="发货管理", Url="/Member/Express?SystemMenu=Menu"}
                }
                });
                ViewBag.SelfNewOrderCount = service.GetSelfNewOrderCount();
            }

            Session["Menu"] = menuGroups;
            return View();
        }

        /// <summary>
        /// 功能选择界面
        /// </summary>
        /// <returns></returns>
        public PartialViewResult FunctionPart()
        {
            return PartialView();
        }

        /// <summary>
        /// 查询出数据显示在菜单栏目中
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadMenuData()
        {
            var menuGroups = new List<MenuGroup>();
            UserSummary user = Session["CurUser"] as UserSummary;
            if (user.UserRoleID == (int)enmRoles.Admin)
            //if (user.Account.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                menuGroups.Add(new MenuGroup()
                {
                    Name = "系统管理",
                    MenuItems = new List<MenuItem>() {
                        new MenuItem(){  Icon = "icon-sys",Id="MemberManage", Name="代理管理", Url="/Member/MemberManage?SystemMenu=Menu"},
                        new MenuItem(){  Icon = "icon-sys",Id="MemberOrderManage", Name="代理订单管理", Url="/Member/MemberOrderManage?SystemMenu=Menu"}
                        //,
                        //new MenuItem(){  Icon = "icon-sys",Id="updateMemberStore", Name="专卖店代理管理", Url="/Member/UpdateMemberStore?SystemMenu=Menu"},
                        //new MenuItem(){  Icon = "icon-sys",Id="manrole", Name="部门信息", Url="/Member/DepartInfo?SystemMenu=Menu"}
                }
                });
            }
            //else if (user.RoleID == (int)enmRoles.Operator)
            //{
            //    menuGroups.Add(new MenuGroup()
            //    {
            //        Name = "新人信息核对",
            //        MenuItems = new List<MenuItem>() { 
            //        new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="新人信息核对", Url="/Member/Check?SystemMenu=Menu"},
            //        new MenuItem(){  Icon = "icon-sys",Id="manrole", Name="推荐代理位置", Url="/Member/RecommendMemberLocation?SystemMenu=Menu"},
            //        new MenuItem(){  Icon = "icon-sys",Id="manrole", Name="部门信息", Url="/Member/DepartInfo?SystemMenu=Menu"}
            //    }
            //    });
            //}
            else if (user.UserRoleID == (int)enmRoles.General)
            {
                menuGroups.Add(new MenuGroup()
                {
                    Name = "系统管理",
                    MenuItems = new List<MenuItem>() {
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="个人资料", Url="/Member/PersonalInfo1?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="订单管理", Url="/Member/PersonalOrder?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="下级代理订单管理", Url="/Member/ChildMemberOrder?SystemMenu=Menu"},
                    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="直属下级代理管理", Url="/Member/DirectChildMember?SystemMenu=Menu"}
                    
                    //,
                    //new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="老代理补单", Url="/Achieve/OldMember?SystemMenu=Menu"},
                    //new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="老代理个人消费", Url="/Achieve/OldMemberConsume?SystemMenu=Menu"},
                    //new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="报单历史", Url="/AchieveHistory/Index?SystemMenu=Menu"},
                }
                });

                //menuGroups.Add(new MenuGroup()
                //{
                //    Name = "业务员管理",
                //    MenuItems = new List<MenuItem>() { 
                //    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="业务员资料", Url="/Member/Index?SystemMenu=Menu"},
                //    new MenuItem(){  Icon = "icon-sys",Id="manrole", Name="推荐代理位置", Url="/Member/RecommendMemberLocation?SystemMenu=Menu"},
                //    new MenuItem(){  Icon = "icon-sys",Id="manrole", Name="部门信息", Url="/Member/DepartInfo?SystemMenu=Menu"}
                //}
                //});
                //menuGroups.Add(new MenuGroup()
                //{
                //    Name = "专卖店信息管理",
                //    MenuItems = new List<MenuItem>() { 
                //    new MenuItem(){  Icon = "icon-sys",Id="manusl", Name="专卖店资料", Url="/UserDetails/Index?SystemMenu=Menu"},
                //}
                //});
            }
            return Json(menuGroups, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult NotSupport()
        {
            return PartialView();
        }

        public ActionResult Logout()
        {
            this.GetService<IUserLogService>().AddLog(UserOperations.Logout);
            this.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

    }
}
