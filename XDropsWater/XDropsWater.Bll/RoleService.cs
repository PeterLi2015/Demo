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
    public class RoleService : IRoleService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsRoleExist(string name)
        {
            Repository<RoleEntity> repo = new Repository<RoleEntity>(uow);
            var rol = repo.FindBy(p => p.Name == name).FirstOrDefault();
            return rol != null;
        }

        /// <summary>
        /// 判断票据角色权限是否存在
        /// </summary>
        /// <param name="name">被检验的名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public bool IsPermissionShortNameExist(string permissionShortName)
        {
            Repository<RoleEntity> repo = new Repository<RoleEntity>(uow);
            var bill = repo.FindBy(p => p.Permissions.Any(p1 => p1.ShortName == permissionShortName)).FirstOrDefault();
            return bill != null;
        }

        private string GetRolePermissionString(RoleEntity role)
        {
            string str = string.Empty;

            if (role.Permissions != null)
            {
                foreach (var per in role.Permissions)
                {
                    str += per.Name + ",";
                }
                if (str.Length > 0)
                {
                    str = str.Remove(str.Length - 1);
                }
            }
            return str;
        }

        /// <summary>
        /// 从数据库获取所有权限的数据
        /// </summary>
        /// <returns></returns>
        public List<RoleSummary> GetAll()
        {
            Repository<RoleEntity> repo = new Repository<RoleEntity>(uow);
            var roleSumList = new List<RoleSummary>();
            foreach (var role in repo.GetAll().ToList())
            {
                var roleSum = new RoleSummary()
                {
                    ID = role.ID,
                    Name = role.Name,
                    Description = role.Description,
                    Permissions = GetRolePermissionString(role)
                };
                roleSumList.Add(roleSum);
            }

            return roleSumList;
        }

        /// <summary>
        /// 从数据库获取某条权限的数据
        /// </summary>
        /// <param name="id">要获取数据的权限编号</param>
        /// <returns></returns>
        public RoleDetail GetDetail(Guid id)
        {
            Repository<RoleEntity> repo = new Repository<RoleEntity>(uow);
            var role = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (role != null)
            {
                var roleDetail = new RoleDetail()
                {
                    ID = role.ID,
                    Name = role.Name,
                    Description = role.Description,
                };
                roleDetail.Permissions = new List<Permission>();
                foreach (var perEntity in role.Permissions)
                {
                    roleDetail.Permissions.Add(new Permission()
                    {
                        ID = perEntity.ID,
                        ShortName = perEntity.ShortName,
                        Description = perEntity.Description,
                        Name = perEntity.Name,
                    });
                }
                return roleDetail;
            }
            else
            {
                return null;
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
            Repository<RoleEntity> roleRepo = new Repository<RoleEntity>(uow);
            var rol = roleRepo.FindBy(p => p.ID == id).FirstOrDefault();
            if (rol != null)
            {
                roleRepo.Remove(rol);
                uow.Commit();
                return ErrorCodes.Successed;
            }
            else
            {
                return ErrorCodes.NotExist;
            }
        }



        public ErrorCodes UpdateRole(Guid roleId, string name, string description, string permissionList)
        {
            try
            {
                var roleRepo = new Repository<RoleEntity>(uow);
                var permissionRepo = new Repository<PermissionEntity>(uow);
                var permissionEntiyList = new List<PermissionEntity>();
                if (permissionList.Length > 0)
                {
                    var idStrList = permissionList.Split(',');
                    foreach (var idStr in idStrList)
                    {
                        var per = permissionRepo.Find(int.Parse(idStr));
                        if (per != null)
                        {
                            permissionEntiyList.Add(per);
                        }
                    }
                }
                var role = roleRepo.Find(roleId);
                if (role == null)
                {
                    role = new RoleEntity();
                    roleRepo.Add(role);
                    role.Permissions = permissionEntiyList;
                }
                else
                {
                    role.Permissions.Clear();
                    role.Permissions.AddRange(permissionEntiyList);
                }
                role.Name = name;
                role.Description = description;

                uow.Commit();

            }
            catch (Exception)
            {
                return ErrorCodes.Exception;
            }

            return ErrorCodes.Successed;
        }


    }
}
