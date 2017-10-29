using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuoBang.Model;

namespace DuoBang.Bll.Interface
{
    public interface IRoleService : IService
    {
        List<RoleSummary> GetAll();

        RoleDetail GetDetail(Guid id);

        ErrorCodes Delete(Guid id);
        bool IsRoleExist(string name);
        bool IsPermissionShortNameExist(string PermissionShortName);
        ErrorCodes UpdateRole(Guid roleId, string name, string description, string permissionList);
    }
}
