using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.DataAccess;

namespace XDropsWater.Bll
{
    /// <summary>
    /// 业务逻辑层-用户操作日志管理
    /// </summary>
    public class UserLogService : IUserLogService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        /// <summary>
        /// 属性：当前用户
        /// </summary>
        public UserSummary CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// 根据用户操作日志编号在数据库中删除相应的用户操作日志记录
        /// </summary>
        /// <param name="id">需要删除数据的用户操作日志编号</param>
        /// <returns>删除成功或错误信息</returns>
        public ErrorCodes Delete(Guid id)
        {
            Repository<UserLogEntity> repo = new Repository<UserLogEntity>(uow);
            var bus = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (bus != null)
            {
                repo.Remove(bus);
                uow.Commit();
                return ErrorCodes.Successed;
            }
            else
            {
                return ErrorCodes.NotExist;
            }
        }


        public void AddLog(UserOperations op, params object[] paramenters)
        {
            if(this.CurrentUser!=null)
            {
                Repository<UserLogEntity> repo = new Repository<UserLogEntity>(uow);
                UserLogEntity log = new UserLogEntity();
                log.CreateOn = DateTime.Now;
                log.Comment = string.Format(this.GetOperationMessageFormat(op), paramenters);
                log.Name = this.CurrentUser.UserName;
                log.Account = this.CurrentUser.Account;
                log.Operation = (int)op;
                repo.Add(log);
                uow.Commit();
            }
        }

        private string GetOperationMessageFormat(UserOperations op)
        {
            string msgFormat = string.Empty;
            switch (op)
            {
                case UserOperations.Login:
                    msgFormat = "成功登录";
                    break;
                case UserOperations.Logout:
                    msgFormat = "成功登出";
                    break;
                case UserOperations.ChangePassword:
                    msgFormat = "成功修改密码";
                    break;
                default:
                    break;
            }
            return msgFormat;
        }

        private string GetOperationString(int op)
        {
            switch ((UserOperations)op)
            {
                case UserOperations.Login:
                    return "登录";
                case UserOperations.Logout:
                    return "退出";
                case UserOperations.ChangePassword:
                    return "修改密码";
                default:
                    return "未知操作";
            }
        }

        /// <summary>
        /// 查找指定日期内的操作日志
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public List<UserLog> Search(DateTime? dateFrom, DateTime? dateTo, int pageIndex, int pageSize, out int total)
        {
            Repository<UserLogEntity> repo = new Repository<UserLogEntity>(uow);
            DateTime minDate = dateFrom.HasValue ? dateFrom.Value : DateTime.MinValue;
            DateTime maxDate = dateTo.HasValue ? dateTo.Value : DateTime.MaxValue;
            var list = repo.GetPaged<DateTime>(pageIndex, pageSize, p => p.CreateOn.Value >= minDate && p.CreateOn.Value <= maxDate, p => p.CreateOn.Value, false, out total)
                       .Select(p => new UserLog { ID = p.ID, Account = p.Account, Comment = p.Comment, Name = p.Name, CreateOn = p.CreateOn.Value, Operation = this.GetOperationString(p.Operation) });
            return list.ToList();
        }
    }
}
