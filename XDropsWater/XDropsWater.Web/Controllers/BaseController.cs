using XDropsWater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XDropsWater.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (this.Session["CurUser"] == null && !(filterContext.Controller is RegisterController)
                && !(filterContext.Controller is VideoController))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    if (filterContext.HttpContext.Request["SystemMenu"] == "Menu")
                    {
                        filterContext.Result = Content("<script>$.messager.alert('重新登录', '由于您长时间未操作，系统需要重新登录', 'info', function () {window.location.href = \"../Login/Logout\";});</script>");
                    }
                    else
                    {
                        filterContext.Result = new ContentResult() { Content = ExecuteResult.Login.ToString() };
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
