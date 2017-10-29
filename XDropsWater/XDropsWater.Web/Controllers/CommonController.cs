using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Model;

namespace XDropsWater.Web.Controllers
{
    public class CommonController : BaseController
    {
        //
        // GET: /Common/

        public ActionResult Index()
        {
            return View();
        }

        public string SessionExpire()
        {
            return ExecuteResult.Success.ToString();
        }
    }
}
