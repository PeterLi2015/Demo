using XDropsWater.Bll.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XDropsWater.Web.Controllers
{
    public class AchieveHistoryController : BaseController
    {
        readonly IAchieveService service;
        public AchieveHistoryController(IAchieveService service)
        {
            this.service = service;
        }
        //
        // GET: /AchieveHistory/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult History(string cardNo, string dateFrom, string dateTo, int type, int page = 1, int rows = 10)
        {
            int total = 0;
            var list = service.History(cardNo, dateFrom, dateTo, type, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public string AchieveHistoryTotal(string batchNo, string cardNo, string dateFrom, string dateTo)
        {
            int bNo = 0;
            int.TryParse(batchNo, out bNo);
            var total = service.AchieveHistoryTotal(cardNo, bNo, dateFrom, dateTo);
            return total.ToString();
        }
    }
}
