using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DuoBang.Bll.Interface;
using DuoBang.Model;

namespace DuoBang.Web.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 验证角色是否存在
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult IsRoleExist(string name)
        {
            var service = this.GetService<IRoleService>();
            var isExist = service.IsRoleExist(name);
            return Json(isExist);
        }
        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应的所有数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult List()
        {
            var service = this.GetService<IRoleService>();
            var role = service.GetAll();
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应编号为id的数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult GetDetail(Guid id)
        {
            var service = this.GetService<IRoleService>();
            var role = service.GetDetail(id);
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Remove(Guid id)
        {
            var service = this.GetService<IRoleService>();
            var errCode = service.Delete(id);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据view传入的值在对应的数据库中保存某条数据的信息，其信息通过传入的jsonStr的反序列化进行识别
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveRole(Guid roleId, string name, string description, string permissionList)
        {
            var service = this.GetService<IRoleService>();
            var errCode = service.UpdateRole(roleId, name, description, permissionList);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }

    }
}
