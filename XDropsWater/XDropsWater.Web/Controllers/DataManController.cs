using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Model;
using XDropsWater.Bll.Interface;
using System.IO;

namespace XDropsWater.Web.Controllers
{
    /// <summary>
    /// 数据字典共用的一个controller，用于增删改查
    /// </summary>
    public class DataManController : BaseController
    {
        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应的所有数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult List(string ser)
        {
            var service = this.GetDataManService(ser);
            var deps = service.GetAll();
            return Json(deps, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应编号为id的数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult Get(string ser, Guid id)
        {
            var service = this.GetDataManService(ser);
            return Json(service.Get(id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据view传入的ser在对应的数据库删除对应编号为id的数据
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Remove(string ser, Guid id)
        {
            var service = this.GetDataManService(ser);
            var errCode = service.Delete(id);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) });
        }

        /// <summary>
        /// 根据view传入的ser在对应的数据库中保存某条数据的信息，其信息通过传入的jsonStr的反序列化进行识别
        /// </summary>
        /// <param name="ser"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(string ser, string jsonStr)
        {
            var service = this.GetDataManService(ser);
            var js = new JsonSerializer();
            var stringReader = new StringReader(jsonStr);
            var jsonReader = new JsonTextReader(stringReader);
            var data = js.Deserialize(jsonReader, this.GetDataManParaType(ser));

            ErrorCodes errCode = service.AddOrUpdate(data);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) });
        }
    }
}