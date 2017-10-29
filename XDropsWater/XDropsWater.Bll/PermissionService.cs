using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoBang.Bll.Interface;
using DuoBang.Model;
using DuoBang.Dal.Entity;
using DuoBang.SC.DataAccess.Interface;
using DuoBang.SC.DataAccess;


namespace DuoBang.Bll
{
    public class PermissionService : IPermissionService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        /// <summary>
        /// 判断权限是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsPermissionExist(string name)
        {
            Repository<PermissionEntity> repo = new Repository<PermissionEntity>(uow);
            var per = repo.FindBy(p => p.Name == name).FirstOrDefault();
            return per != null;
        }

        /// <summary>
        /// 从数据库获取所有权限的数据
        /// </summary>
        /// <returns></returns>
        public List<object> GetAll()
        {
            Repository<PermissionEntity> repo = new Repository<PermissionEntity>(uow);
            var pers = from per in repo.GetAll()
                       select new Permission() { ID = per.ID, ShortName = per.ShortName, Name = per.Name, Description = per.Description };
            return pers.ToList<object>();
        }

        /// <summary>
        /// 从数据库获取某条权限的数据
        /// </summary>
        /// <param name="id">要获取数据的权限编号</param>
        /// <returns></returns>
        public object Get(Guid id)
        {
            Repository<PermissionEntity> repo = new Repository<PermissionEntity>(uow);
            var per = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (per != null)
            {
                return new Permission() { ID = per.ID, ShortName = per.ShortName, Name = per.Name, Description = per.Description };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 在数据库中添加或更新权限数据
        /// </summary>
        /// <param name="model">需要添加或删除的权限对象</param>
        /// <returns>操作的成功或错误信息</returns>
        public ErrorCodes AddOrUpdate(object model)
        {
            try
            {
                Permission permission = model as Permission;
                Repository<PermissionEntity> repo = new Repository<PermissionEntity>(uow);
                var per = repo.FindBy(p => p.ID == permission.ID).FirstOrDefault();
                if (per != null)
                {
                    per.ShortName = permission.ShortName;
                    per.Name = permission.Name;
                    per.Description = permission.Description;
                }
                else
                {
                    repo.Add(new PermissionEntity() { ShortName = permission.ShortName, Name = permission.Name, Description = permission.Description });
                }
                uow.Commit();
                return ErrorCodes.Successed;
            }
            catch (Exception)
            {
                return ErrorCodes.Exception;
            }
        }

        /// <summary>
        /// 属性：当前用户
        /// </summary>
        public UserSummary CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// 根据业务分类编号在数据库中删除相应的业务分类记录
        /// </summary>
        /// <param name="id">需要删除数据的业务分类编号</param>
        /// <returns>删除成功或错误信息</returns>
        public ErrorCodes Delete(Guid id)
        {
            Repository<PermissionEntity> repo = new Repository<PermissionEntity>(uow);
            var per = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (per != null)
            {
                repo.Remove(per);
                uow.Commit();
                return ErrorCodes.Successed;
            }
            else
            {
                return ErrorCodes.NotExist;
            }
        }
    }
}
