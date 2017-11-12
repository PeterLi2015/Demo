using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Bll;
using XDropsWater.Model;
using XDropsWater.Bll.Interface;
using System.IO;

namespace XDropsWater.Web.Controllers
{
    public class SystemManagmentController : BaseController
    {
        //
        // GET: /SystemManagment/

        public PartialViewResult RoleManagment()
        {
            return PartialView();
        }
        public PartialViewResult PermissionManagment()
        {
            return PartialView();
        }
        public PartialViewResult UserManagment()
        {
            return PartialView();
        }
        //public JsonResult IsPermissionExist(string Id)
        //{
        //    var service = this.GetService<IPermissionService>();
        //    var isExist = service.Get(Guid.Parse(Id));
        //    return Json(isExist);
        //}
        //public JsonResult IsPermissionNameExist(string PermissionName)
        //{
        //    var service = this.GetService<IPermissionService>();
        //    var isExist = service.IsPermissionExist(PermissionName);
        //    return Json(isExist);
        //}

        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应的所有数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        //public JsonResult List(string ser)
        //{
        //    var service = this.GetService<IRoleService>();
        //    var role = service.GetAll();
        //    return Json(role, JsonRequestBehavior.AllowGet);
        //}
    }
}
