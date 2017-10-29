using XDropsWater.Bll;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XDropsWater.Web
{
    public class LoggingInterceptionBehavior : IInterceptionBehavior
    {
        public bool WillExecute
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var user = HttpContext.Current.Session["CurUser"] as UserSummary;
            var service = input.Target as IService;
            service.CurrentUser = user;

            var result = getNext()(input, getNext);

            
            if (result.Exception != null)
            {
                ILogService log = new LogService();
                LogModel model = new LogModel();
                
                model.CreateBy = user != null ? user.ID : Guid.Empty;
                model.CreateOn = DateTime.Now;
                model.ErrMsg = result.Exception.Message + "; " + GetInnerErrMsg(result.Exception);
                model.ID = new Guid();
                model.ModuleName = string.Format("{0} - {1}", input.MethodBase.DeclaringType, input.MethodBase);
                log.WriteLog(model);
            }

            return result;
        }

        private string GetInnerErrMsg(Exception ex)
        {
            if (ex.InnerException != null)
                return ex.InnerException.Message + "; " + GetInnerErrMsg(ex.InnerException);
            return "";
        }
    }
}