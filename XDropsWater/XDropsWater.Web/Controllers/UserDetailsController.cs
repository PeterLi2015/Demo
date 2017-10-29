using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;

namespace XDropsWater.Web.Controllers
{
    public class UserDetailsController : Controller
    {
        readonly IUserService service;
        public UserDetailsController(IUserService service)
        {
            this.service = service;
        }
        //
        // GET: /UserDetails/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Get()
        {
            var user = service.Get();
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        public string Save(string userName, string telPhone, string mobi,
            string fax, string addr)
        {
            return service.Save(userName,telPhone,mobi,fax,addr);
        }
    }
}
