using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuoBang.Bll;
using DuoBang.Model;
using DuoBang.Bll.Interface;
using Newtonsoft.Json;
using System.IO;

namespace DuoBang.Web.Controllers
{
    public class PermissionController : Controller
    {
        //
        // GET: /Permission/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult IsPermissionExist(string Id)
        {
            var service = this.GetService<IPermissionService>();
            var isExist = service.Get(new Guid(Id));
            return Json(isExist);
        }
        public JsonResult IsPermissionNameExist(string PermissionName)
        {
            var service = this.GetService<IPermissionService>();
            var isExist = service.IsPermissionExist(PermissionName);
            return Json(isExist);
        }

    }
}
