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
    /// <summary>
    /// 业务逻辑层-部门管理
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        /// <summary>
        /// 从数据库获取所有部门的数据
        /// </summary>
        /// <returns></returns>
        public List<object> GetAll()
        {
            Repository<DepartmentEntity> repo = new Repository<DepartmentEntity>(uow);
            var deps = from dep in repo.GetAll()
                       select new Department() { ID = dep.ID, Name = dep.Name, Description = dep.Description };
            return deps.ToList<object>();
        }

        /// <summary>
        /// 从数据库获取某条部门的数据
        /// </summary>
        /// <param name="id">要获取数据的部门编号</param>
        /// <returns></returns>
        public object Get(Guid id)
        {
            Repository<DepartmentEntity> repo = new Repository<DepartmentEntity>(uow);
            var dep = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (dep != null)
            {
                return new Department() { ID = dep.ID, Name = dep.Name, Description = dep.Description, EntityStatus = dep.EntityStatus };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断部门是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsDeparmentExist(string name)
        {
            Repository<DepartmentEntity> repo = new Repository<DepartmentEntity>(uow);
            var dep = repo.FindBy(p => p.Name == name).FirstOrDefault();
            return dep != null;
        }

        /// <summary>
        /// 在数据库中添加或更新部门数据
        /// </summary>
        /// <param name="model">需要添加或删除的部门数据对象</param>
        /// <returns>操作的成功或错误信息</returns>
        public ErrorCodes AddOrUpdate(object model)
        {
            try
            {
                Department department = model as Department;
                Repository<DepartmentEntity> repo = new Repository<DepartmentEntity>(uow);
                var dep = repo.FindBy(p => p.ID == department.ID).FirstOrDefault();
                if (dep != null)
                {
                    dep.Name = department.Name;
                    dep.Description = department.Description;
                }
                else
                {
                    repo.Add(new DepartmentEntity() { Name = department.Name, Description = department.Description, EntityStatus = DeleteStatus.Normal });
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
        /// 根据部门编号在数据库中删除相应的部门记录
        /// </summary>
        /// <param name="id">需要删除数据的部门编号</param>
        /// <returns>删除成功或错误信息</returns>
        public ErrorCodes Delete(Guid id)
        {
            Repository<DepartmentEntity> repo = new Repository<DepartmentEntity>(uow);
            var dep = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (dep != null)
            {
                repo.Remove(dep);
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
