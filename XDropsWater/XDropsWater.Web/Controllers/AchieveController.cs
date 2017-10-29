using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;

namespace XDropsWater.Web.Controllers
{

    public class AchieveController : BaseController
    {
        readonly IAchieveService service;
        readonly IMemberService memberService;
        public AchieveController(IAchieveService service, IMemberService memberService)
        {
            this.service = service;
            this.memberService = memberService;
        }

        //
        // GET: /Achieve/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新代理入单
        /// </summary>
        /// <returns></returns>
        public ActionResult NewMember()
        {
            return View();
        }

        /// <summary>
        /// 老代理补单
        /// </summary>
        /// <returns></returns>
        public ActionResult OldMember()
        {
            return View();
        }

        /// <summary>
        /// 老代理消费
        /// </summary>
        /// <returns></returns>
        public ActionResult OldMemberConsume()
        {
            return View();
        }

        public JsonResult Get(string cardNo, int type)
        {
            var list = service.Get(cardNo, type);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public string SumAchieve(string cardNo, int type)
        {
            return service.SumAchieve(cardNo, type);
        }
        public JsonResult Add(string cardNo, decimal amount)
        {

            AchieveModel model = new AchieveModel();
            model.MemberIdentityCardNo = cardNo;
            model.Amount = amount;
            var errCode = service.Add(model);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }
        public string Delete(Guid id)
        {
            return service.Delete(id);
        }
        [HttpPost]
        public string GetDateFrom()
        {
            var d = DateTime.Now.AddDays(-1);
            return d.ToString("yyyy-MM-dd");
        }
        [HttpPost]
        public string GetDateTo()
        {
            var d = DateTime.Now;
            return d.ToString("yyyy-MM-dd");
        }
        public string CheckIDCard(string cardNo)
        {
            try
            {
                MemberModel m = memberService.Get(cardNo);
                return m.MemberName;
            }
            catch
            {
                return ExecuteResult.Exception.ToString();
            }
        }
        public bool CheckID(string cardNo)
        {
            return memberService.CheckIDCard(cardNo);
        }
        public string Submit(int type)
        {
            return service.Submit(type);
        }

        /// <summary>
        /// 新代理入单
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="memberName"></param>
        /// <param name="achieve"></param>
        /// <returns></returns>
        public string NewMemberAchieveAdd(string cardNo, string memberName, decimal achieve, int score)
        {
            string checkResult = memberService.CheckNewOrOldMember(cardNo, 1);
            if (!string.Equals(checkResult, ExecuteResult.Success.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return checkResult;
            }

            //3.添加代理
            var member = new MemberModel();
            member.IdentityCardNo = cardNo;
            member.MemberName = memberName;
            var errCode = memberService.AddNoExistsMember(member);
            if (errCode != ErrorCodes.Successed) return ExecuteResult.Exception.ToString();

            //4.添加业绩
            AchieveModel model = new AchieveModel();
            model.MemberIdentityCardNo = cardNo;
            model.Amount = achieve;
            model.Type = 1;
            model.Score = score;
            var errCode1 = service.Add(model);
            if (errCode1 != ErrorCodes.Successed) return ExecuteResult.Exception.ToString();

            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 老代理补单
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="memberName"></param>
        /// <param name="achieve"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public string OldMemberAchieveAdd(string cardNo, string memberName, decimal achieve, int score)
        {
            string checkResult = memberService.CheckNewOrOldMember(cardNo, 2);
            if (!string.Equals(checkResult, ExecuteResult.Success.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return checkResult;
            }

            //3.添加业绩
            AchieveModel model = new AchieveModel();
            model.MemberIdentityCardNo = cardNo;
            model.Amount = achieve;
            model.Type = 2;
            model.Score = score;
            var errCode1 = service.Add(model);
            if (errCode1 != ErrorCodes.Successed) return ExecuteResult.Exception.ToString();

            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 老代理消费
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="memberName"></param>
        /// <param name="achieve"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public string OldMemberConsumeAdd(string cardNo, string memberName, decimal achieve)
        {
            string checkResult = memberService.CheckNewOrOldMember(cardNo, 3);
            if (!string.Equals(checkResult, ExecuteResult.Success.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return checkResult;
            }

            //3.添加业绩
            AchieveModel model = new AchieveModel();
            model.MemberIdentityCardNo = cardNo;
            model.Amount = achieve;
            model.Type = 3;
            var errCode1 = service.Add(model);
            if (errCode1 != ErrorCodes.Successed) return ExecuteResult.Exception.ToString();

            return ExecuteResult.Success.ToString();
        }
    }
}
