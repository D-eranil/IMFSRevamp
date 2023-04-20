using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System.Collections.Generic;

namespace IMFS.BusinessLogic.RoleManagement
{
    public interface IRoleManager
    {
        List<Web.Models.DBModel.AspNetRoles> GetRoles();
        AspNetRoles GetRoleById(string roleId);
        AspNetRoles GetUserRole(string userId);
        ErrorModel AddUserToRole(string userId, string roleName);
    }
}
