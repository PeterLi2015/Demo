using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.DataAccess;
using Unity.Attributes;
using System.Net;

namespace XDropsWater.Bll
{
    public class UserService : BaseService, IUserService, IRegisterObserver
    {
        [Dependency]
        public IUnitOfWork Uow { get; set; }

        [Dependency]
        public IRepository<UserEntity> UserDb { get; set; }

        public string SaveNewPassword(string oldPassowrd, string newPassword)
        {
            var checkOldPassword = IsValidUser(oldPassowrd);
            if (checkOldPassword != ExecuteResult.Success.ToString())
            {
                return checkOldPassword;
            }
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var user = repo.FindBy(p => p.ID == this.CurrentUser.ID).FirstOrDefault();
            user.Password = user.SwitchEncryptDecrypt(newPassword);
            repo.Update(user);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }
        public string IsValidUser(string password)
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            if (!repo.FindBy(p => p.Account == this.CurrentUser.Account && p.Password == p.SwitchEncryptDecrypt(password)).Any())
            {
                return "原始密码无效";
            }
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 检验登录用户是否合法
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsValidUser(string account, string password)
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var act = repo.FindBy(p => p.Account == account && p.Password == p.SwitchEncryptDecrypt(password)).FirstOrDefault();
            return act != null;
        }


        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsUserExist(string account)
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var usr = repo.FindBy(p => p.Account == account).FirstOrDefault();
            return usr != null;
        }


        /// <summary>
        /// 从数据库获取所有权限的数据
        /// </summary>
        /// <returns></returns>
        public List<UserSummary> GetAll()
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var userSumList = new List<UserSummary>();
            foreach (var user in repo.GetAll().ToList())
            {
                var userSum = new UserSummary()
                {
                    ID = user.ID,
                    UserName = user.UserName,
                    Account = user.Account,
                    Password = user.Password
                    //DepartmentId = user.Department.ID,
                    //DepartmentName = user.Department.Name,
                    //Roles = GetUserRoleString(user)
                };
                userSumList.Add(userSum);
            }
            return userSumList;
        }

        public UserSummary GetUser(string account)
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var usr = repo.FindBy(p => p.Account == account).FirstOrDefault();

            if (usr != null)
            {
                Repository<MemberEntity> member = new Repository<MemberEntity>(Uow);
                var mem = member.FindBy(o => o.ID == usr.MemberID).FirstOrDefault();
                Repository<SystemConfigEntity> sys = new Repository<SystemConfigEntity>(Uow);
                var price = sys.FindBy(o => o.Name.Equals("price", StringComparison.CurrentCultureIgnoreCase)).First().ConfigValue;
                mem.CurrentRoleQuantity = mem.CurrentRoleAmount / decimal.Parse(price);
                mem.CurrentRoleQuantity = Math.Round(mem.CurrentRoleQuantity, 4);
                return new UserSummary()
                {
                    UserName = usr.UserName,
                    Account = usr.Account,
                    Password = usr.Password,
                    UserRoleID = usr.UserRoleID,
                    ID = usr.ID,
                    MemberID = usr.MemberID.HasValue ? usr.MemberID.Value : Guid.Empty,
                    RoleID = mem != null ? mem.RoleID : -1,
                    CurrentRoleQuantity = mem.CurrentRoleQuantity

                };
            }
            else
            {
                return null;
            }
        }

        public UserDetail Get()
        {
            var repo = new Repository<UserEntity>(Uow);
            var user = repo.FindBy(p => p.ID == this.CurrentUser.ID).FirstOrDefault();
            if (user != null)
            {
                return new UserDetail()
                {
                    ID = user.ID,
                    Name = user.UserName,
                    //MobTel = user.MobTel,
                    //Telphone = user.Telphone,
                    //Sex = user.Sex,
                    Account = user.Account,
                    //Address = user.Address,
                    //Fax = user.Fax
                };

            }
            else return null;
        }

        /// <summary>
        /// 从数据库获取某条权限的数据
        /// </summary>
        /// <param name="id">要获取数据的权限编号</param>
        /// <returns></returns>
        public UserDetail GetDetail(Guid id)
        {
            Repository<UserEntity> repo = new Repository<UserEntity>(Uow);
            var user = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (user != null)
            {
                var userDetail = new UserDetail()
                {
                    ID = user.ID,
                    Name = user.UserName,
                    //MobTel = user.MobTel,
                    //Telphone = user.Telphone,
                    //Sex = user.Sex,
                    Account = user.Account,
                    Password = user.Password,
                    //DepartmentId = user.Department.ID,
                    //DepartmentName = user.Department.Name,
                };
                //userDetail.Roles = new List<RoleSummary>();
                //foreach (var rolEntity in user.Roles)
                //{
                //    userDetail.Roles.Add(new RoleSummary()
                //    {
                //        ID = rolEntity.ID,
                //        Name = rolEntity.Name,
                //        Description = rolEntity.Description,
                //    });
                //}
                return userDetail;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据业务分类编号在数据库中删除相应的业务分类记录
        /// </summary>
        /// <param name="id">需要删除数据的业务分类编号</param>
        /// <returns>删除成功或错误信息</returns>
        public ErrorCodes Delete(Guid id)
        {
            Repository<UserEntity> userRepo = new Repository<UserEntity>(Uow);
            var usr = userRepo.FindBy(p => p.ID == id).FirstOrDefault();
            if (usr != null)
            {
                userRepo.Remove(usr);
                Uow.Commit();
                return ErrorCodes.Successed;
            }
            else
            {
                return ErrorCodes.NotExist;
            }
        }

        /// <summary>
        /// update a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="telphone"></param>
        /// <param name="mobtel"></param>
        /// <param name="sex"></param>
        /// <param name="departmentid"></param>
        /// <param name="department"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        public ErrorCodes UpdateUser(Guid userId, string name, string account, string password, string telphone, string mobtel, int sex, int departmentid, string department, string roleList)
        {
            var userRepo = new Repository<UserEntity>(Uow);

            var user = userRepo.Find(userId);

            user.UserName = name;
            user.Account = account;
            user.Password = password;

            Uow.Commit();

            return ErrorCodes.Successed;
        }

        public string Save(string name, string telphone, string mobtel, string fax, string address)
        {
            var userRepo = new Repository<UserEntity>(Uow);
            var user = userRepo.Find(this.CurrentUser.ID);
            if (user == null)
            {
                return "获取用户信息失败";
            }
            user.UserName = name;

            user.UpdateBy = this.CurrentUser.ID;
            user.UpdateOn = DateTime.Now;
            userRepo.Update(user);
            Uow.Commit();

            return ExecuteResult.Success.ToString();
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
            var repo = new Repository<UserEntity>(Uow);

            UserEntity entity = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (entity == null)
            {
                return "您要修改的专卖店不存在";
            }
            //entity.Address = address;
            entity.UpdateBy = this.CurrentUser.ID;
            entity.UpdateOn = DateTime.Now;
            //entity.Fax = fax;
            //entity.MobTel = mobile;
            entity.UserName = name;
            //entity.Telphone = telephone;
            //entity.Sex = sex;
            repo.Update(entity);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ResetPassword(Guid id)
        {
            var repo = new Repository<UserEntity>(Uow);

            UserEntity entity = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (entity == null)
            {
                return "账号不存在";
            }
            entity.Password = entity.SwitchEncryptDecrypt("111111");
            repo.Update(entity);
            Uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string AccountExists(string account)
        {
            var repo = new Repository<UserEntity>(Uow);
            if (!repo.FindBy(p => p.Account == account).Any())
            {
                return "账号不存在";
            }
            return ExecuteResult.Success.ToString();
        }



        public List<UserSummary> GetStore(string account, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public string AddStore(string account, string name, string sex, string telephone, string mobile, string fax, string address)
        {
            throw new NotImplementedException();
        }

        public List<UserSummary> GetStoreForUpdateMember(string account, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public void Register(RegisterModel model)
        {
            UserEntity user = new UserEntity();
            user.Account = model.Member.Mobile;
            user.CreateBy = model.User.Id;
            user.CreateOn = DateTime.Now;
            user.ID = Guid.NewGuid();
            user.MemberID = model.Member.MemberId;
            user.Password = user.SwitchEncryptDecrypt(GlobalConstants.InitialPassword);
            user.UserName = model.Member.MemberName;
            user.UserRoleID = (int)enmRoles.General;
            UserDb.Add(user);
        }

        public List<UserEntity> DataTransfer()
        {
            
            return UserDb.FindAll<UserEntity>(o => o.UserRoleID == (int)enmRoles.General).ToList();
                
        }

        private string MakeSign(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
        }
    }
}
