using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;

namespace XDropsWater.Web.Controllers
{
    public class UserController : BaseController
    {
        readonly IUserService service;
        readonly IUserLogService userLogService;
        public UserController(IUserService service, IUserLogService userLogService)
        {
            this.service = service;
            this.userLogService = userLogService;
        }
        public string ChangePassword(string oldPassword, string newPassword)
        {
            return service.SaveNewPassword(oldPassword, newPassword);
        }
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 专卖店管理
        /// </summary>
        /// <returns></returns>
        public ActionResult StoreManage()
        {
            return View();
        }



        /// <summary>
        /// 验证帐号是否存在
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult IsUserExist(string UName)
        {
            var isExist = service.IsUserExist(UName);
            //return isExist;
            return new JsonResult { Data = new { valid = isExist } };
        }
        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应的所有数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult List()
        {
            var user = service.GetAll();
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据view传入的ser从对应的数据库中获取对应编号为id的数据
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
        public JsonResult GetDetail(Guid id)
        {
            var user = service.GetDetail(id);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Remove(Guid id)
        {
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
        public JsonResult SaveUser(Guid userId, string name, string account, string password, string telphone, string mobtel, int sex, int departmentid, string department, string roleList)
        {
            var errCode = service.UpdateUser(userId, name, account, password, telphone, mobtel, sex, departmentid, department, roleList);
            return Json(new OpResult() { ErrCode = errCode, ErrMsg = this.GetErrorMessage(errCode) }, JsonRequestBehavior.AllowGet);
        }

        public string GetCurrentUserName()
        {
            return userLogService.CurrentUser.UserName;
        }
        public string GetUserID(string account)
        {
            UserSummary user = service.GetUser(account);
            if (user == null) return "";
            return user.ID.ToString();
        }

        /// <summary>
        /// 获取专卖店列表
        /// </summary>
        /// <param name="account"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetStore(string account, int page = 1, int rows = 10)
        {
            int total = 0;
            List<UserSummary> list = service.GetStore(account, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加专卖店
        /// </summary>
        /// <param name="account"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="telephone"></param>
        /// <param name="mobile"></param>
        /// <param name="fax"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public string AddStore(string account, string name, string sex, string telephone, string mobile,
            string fax, string address)
        {
            return service.AddStore(account, name, sex, telephone, mobile, fax, address);
        }

        /// <summary>
        /// 修改专卖店信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="telephone"></param>
        /// <param name="mobile"></param>
        /// <param name="fax"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public string ModifyStore(Guid id, string name, int sex, string telephone, string mobile,
            string fax, string address)
        {
            return service.ModifyStore(id, name, sex, telephone, mobile, fax, address);

        }

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string AccountExists(string account)
        {
            return service.AccountExists(account);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ResetPassword(Guid id)
        {
            return service.ResetPassword(id);

        }


        /// <summary>
        /// 获取专卖店信息，用来修改代理所属的专卖店
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetStoreForUpdateMember(string account, int page = 1, int rows = 10)
        {
            int total = 0;
            var list = service.GetStoreForUpdateMember(account, ref total, page, rows);
            var json = new
            {
                total = total,
                rows = list.ToArray()
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DataTransfer()
        {
            try
            {
                var users = service.DataTransfer();

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

                //创建HttpClient（注意传入HttpClientHandler）
                using (var http = new HttpClient(handler))
                {
                    var url = "http://www.xsdhbkj.com/api/userinfoapi";
                    long timeSpan = DateTime.Now.Ticks;
                    var appKey = "PG6e31aa765fcb436b";
                    var secret = "D9U7YY5D7FF2748AED89E90HJ88881E6";
                    var sign = MakeSign(appKey + secret + timeSpan);
                    int i = 0;
                    foreach (var user in users)
                    {
                        i++;
                        //使用FormUrlEncodedContent做HttpContent
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                        {
                            {"username", user.Account},
                            {"password", "111111"},
                            {"integration", "10000"},
                            {"timespan", timeSpan.ToString()},
                            {"appkey", appKey},
                            {"sign", sign},
                            {"force_gzip", "1"}
                        });

                        //await异步等待回应
                        var task = http.PostAsync(url, content);
                        task.Wait();
                        Console.WriteLine(task.Result);
                    }
                    Console.WriteLine(i);

                }
            }
            catch(Exception e)
            {
                throw e;
            }
            
            return new EmptyResult();
        }
        private string MakeSign(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
        }
    }
}
