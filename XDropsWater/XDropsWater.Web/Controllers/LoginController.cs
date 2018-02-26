using XDropsWater.CrossCutting.GDI;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Model;
using XDropsWater.Bll.Interface;
using XDropsWater.Web.Attribute;
using System.Web.Security;

namespace XDropsWater.Web.Controllers
{
    [CustomerHandleError]
    public class LoginController : Controller
    {
        readonly IUserService service;
        readonly IUserLogService userLogService;

        public LoginController(IUserService service, IUserLogService userLogService)
        {
            this.service = service;
            this.userLogService = userLogService;
        }
        //
        // GET: /Login/

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult GetVerifyCode()
        {
            VerifyCode code = new VerifyCode();
            code.Length = 4;
            string codeStr = code.CreateVerifyCode();
            Session["LoginValidateCode"] = codeStr;
            var img = code.CreateImageCode(codeStr);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Jpeg);
            return File(stream.ToArray(), @"image/jpeg");
        }

        public ActionResult CheckUserName(string UName)
        {
            var isExist = service.IsUserExist(UName);
            //return isExist;
            return new JsonResult { Data = new { valid = isExist } };
        }

        private Permission GetPermission(UserSummary user)
        {
            var result = new Permission();
            if (user.UserRoleID == (int)enmRoles.All)
            {
                result.MemberManageAdd = true;
                result.MemberManageOperate = true;
            }
            if (user.UserRoleID == (int)enmRoles.All ||
                user.UserRoleID == (int)enmRoles.General)
            {
                result.MemberOrderManageOperate = true;
            }
            if (user.UserRoleID == (int)enmRoles.All ||
                user.UserRoleID == (int)enmRoles.Admin)
            {
                result.IsAdmin = true;
            }
            if(user.UserRoleID == (int)enmRoles.Financial)
            {
                result.IsFinancial = true;
            }
            return result;
        }

        public ActionResult CheckUser(string uname, string pwd)
        {
            var result = new CurrentUser();
            var permission = new Permission();
            string msg = string.Empty;
            //var service = this.GetService<IUserService>();
            if (!service.IsValidUser(uname, pwd))
            {
                throw new Exception("用户名或密码错误");
            }
            else
            {
                this.Session["CurUser"] = service.GetUser(uname);
                permission = GetPermission(Session["CurUser"] as UserSummary);
                userLogService.AddLog(UserOperations.Login);
            }
            result.Permission = permission;
            result.MainUrl = Url.Action("Index", "Home");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            userLogService.AddLog(UserOperations.Logout);
            this.Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        private void AddToCookie(string userName, Guid id, string role = "StoreManager", bool rememberMe = false)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
              userName,
              DateTime.Now,
              rememberMe ? DateTime.Now.AddDays(14) : DateTime.Now.AddMinutes(20),
              false,
              id.ToString() + ";" + role, //注册的用户只能是 StoreManager 角色
              FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

        }
    }
}
