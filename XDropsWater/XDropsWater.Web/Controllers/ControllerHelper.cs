using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDropsWater.Bll.Interface;
using System.Web.Mvc;
using XDropsWater.Model;
using XDropsWater.Bll;

namespace XDropsWater.Web.Controllers
{
    /// <summary>
    /// 数据字典共用的一个controller
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// 根据view中传入的ser来获得对应的Type
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="ser"></param>
        /// <returns></returns>
        public static Type GetDataManParaType(this Controller controller, string ser)
        {
            //if (ser == "dep")
            //{
            //    return typeof(Department);
            //}
            if (ser == "usl")
            {
                return typeof(UserLog);
            }
            //else if (ser == "per")
            //{
            //    return typeof(Permission);
            //}
            //else if (ser == "role")
            //{
            //    return typeof(RoleSummary);
            //}
            else if (ser == "user")
            {
                return typeof(UserSummary);
            }
            else
            {
                throw new NotSupportedException("Not supported service");
            }
        }

        /// <summary>
        /// 根据view中传入的ser来获得对应的Service
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="ser"></param>
        /// <returns></returns>
        public static IDataManService GetDataManService(this Controller controller, string ser)
        {
            IDataManService service = null;

            //if (ser == "dep")
            //{
            //    service = new DepartmentService();
            //}
            //else if (ser == "per")
            //{
            //    service = new PermissionService();
            //}
            //else
            //{
            //    throw new NotSupportedException("Not supported service");
            //}

            if (controller.Session["CurUser"] != null)
            {
                service.CurrentUser = controller.Session["CurUser"] as UserSummary;
            }

            return service;
        }

        /// <summary>
        /// 根据不同的IService生成不同的对应的Service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static TService GetService<TService>(this Controller controller) where TService : class, IService
        {
            TService service = default(TService);

            //if (typeof(TService).Equals(typeof(IDepartmentService)))
            //{
            //    service = new DepartmentService() as TService;
            //}
            if (typeof(TService).Equals(typeof(IUserLogService)))
            {
                service = new UserLogService() as TService;
            }
            //else if (typeof(TService).Equals(typeof(IPermissionService)))
            //{
            //    service = new PermissionService() as TService;
            //}
            //else if (typeof(TService).Equals(typeof(IRoleService)))
            //{
            //    service = new RoleService() as TService;
            //}
            else if (typeof(TService).Equals(typeof(IUserService)))
            {
                service = new UserService() as TService;
            }
            else if (typeof(TService).Equals(typeof(IUserLogService)))
            {
                service = new UserLogService() as TService;
            }
            else if (typeof(TService).Equals(typeof(IAchieveService)))
            {
                service = new AchieveService() as TService;
            }
            else if (typeof(TService).Equals(typeof(IMemberService)))
            {
                service = new MemberService() as TService;
            }
            else
            {
                throw new NotSupportedException("Not supported service");
            }

            if (controller.Session["CurUser"] != null)
            {
                service.CurrentUser = controller.Session["CurUser"] as UserSummary;
            }

            return service;
        }

        /// <summary>
        /// 根据errCode返回成功或错误信息
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public static string GetErrorMessage(this Controller controller, ErrorCodes errCode)
        {
            return GetErrorMessage(errCode);
        }

        public static string GetErrorMessage(ErrorCodes errCode)
        {
            switch (errCode)
            {
                case ErrorCodes.Successed:
                    return "操作成功！";
                case ErrorCodes.Exception:
                    return "操作发生异常！";
                case ErrorCodes.NotExist:
                    return "要处理的记录不存在！";
                case ErrorCodes.Duplicated:
                    return "要处理的记录已经存在！";
                case ErrorCodes.WrongVerifyCode:
                    return "验证码错误，点击验证码，换个验证码试试！";
                case ErrorCodes.AccountPasswordNotMatch:
                    return "账号和密码不匹配！";
                //case ErrorCodes.NotValidDateTime:
                //    return "请输入有效日期!";
                case ErrorCodes.HasBeenUsed:
                    return "此记录在别处被使用，请清除后再次进行操作!";
                case ErrorCodes.ParentNotExists:
                    return "上级身份证号码输入有误，请重新输入！";
                case ErrorCodes.IDCardError:
                    return "身份证号码输入有误，是否重新输入？";
                case ErrorCodes.MemberNotExist:
                    return "该代理不存在，是否添加？";
                default:
                    return "未知错误！";
            }
        }

    }
}