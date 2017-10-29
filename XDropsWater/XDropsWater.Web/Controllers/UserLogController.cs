using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Bll;
using XDropsWater.Bll.Interface;

namespace XDropsWater.Web.Controllers
{
    public class UserLogController : BaseController
    {
        readonly IUserLogService service;
        public UserLogController(IUserLogService service)
        {
            this.service = service;
        }
        //
        // GET: /UserLog/

        public PartialViewResult DataDictionary()
        {
            return PartialView();
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="dateFrom">开始时间</param>
        /// <param name="dateTo">结束时间</param>
        /// <param name="page">第几页，从1开始</param>
        /// <param name="rows">每页数量</param>
        /// <returns>要查询页的数据和总记录数</returns>
        public JsonResult Search(string dateFrom, string dateTo, int page, int rows)
        {
            DateTime? fromDate = null;
            if (!string.IsNullOrWhiteSpace(dateFrom))
            {
                fromDate = DateTime.Parse(dateFrom);
            }
            DateTime? toDate = null;
            if (!string.IsNullOrWhiteSpace(dateTo))
            {
                toDate = DateTime.Parse(dateTo);
            }

            int total;
            var logs = service.Search(fromDate, toDate, page - 1, rows, out total);

            var dataPackage = new Dictionary<string, object>();
            dataPackage.Add("total", total);
            dataPackage.Add("rows", logs);

            return Json(dataPackage, JsonRequestBehavior.AllowGet);
        }
    }
}
