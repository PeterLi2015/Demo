using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Attributes;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.Model;

namespace XDropsWater.Bll.Interface
{
    public interface IUserService : IService
    {
        //[Dependency]
        //IUnitOfWork Uow { get; set; }

        //[Dependency]
        //IRepository<UserEntity> UserDb { get; set; }

        string SaveNewPassword(string oldPassword, string newPassword);
        string IsValidUser(string password);
        List<UserSummary> GetAll();

        UserDetail GetDetail(Guid id);
        UserSummary GetUser(string account);
        UserDetail Get();
        ErrorCodes Delete(Guid id);
        bool IsUserExist(string account);
        bool IsValidUser(string account, string password);
        ErrorCodes UpdateUser(Guid userId, string name, string account, string password, string telphone, string mobtel, int sex, int departmentid, string department, string roleList);
        string Save(string name, string telphone, string mobtel, string fax, string address);
        List<UserSummary> GetStore(string account, ref int total, int page = 1, int rows = 10);

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
        string AddStore(string account, string name, string sex, string telephone, string mobile,
            string fax, string address);

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        string AccountExists(string account);

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
        string ModifyStore(Guid id, string name, int sex, string telephone, string mobile,
            string fax, string address);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string ResetPassword(Guid id);

        /// <summary>
        /// 获取专卖店列表，用来修改代理所属的专卖店信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        List<UserSummary> GetStoreForUpdateMember(string account, ref int total, int page = 1, int rows = 10);

        List<UserEntity> DataTransfer();
    }
}
