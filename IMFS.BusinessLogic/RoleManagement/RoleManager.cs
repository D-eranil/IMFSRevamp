using IMFS.DataAccess.Repository;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMFS.BusinessLogic.RoleManagement
{
    public class RoleManager : IRoleManager
    {
        private readonly IRepository<AspNetRoles> _aspNetRolesRepository;
        private readonly IRepository<AspNetUserRoles> _aspNetUserRolesRepository;

        public RoleManager(            
            IRepository<AspNetRoles> aspNetRolesRepository,
            IRepository<AspNetUserRoles> aspNetUserRolesRepository
            )
        {            
            _aspNetRolesRepository = aspNetRolesRepository;
            _aspNetUserRolesRepository = aspNetUserRolesRepository;

        }

        public List<AspNetRoles> GetRoles()
        {
            return _aspNetRolesRepository.Table.ToList();
        }
        public AspNetRoles GetRoleById(string roleId)
        {
            return _aspNetRolesRepository.Table.Where(x => x.Id == roleId).FirstOrDefault();
        }

        public AspNetRoles GetUserRole(string userId)
        {
            var roleDetails = (from userRole in _aspNetUserRolesRepository.Table
                               join role in _aspNetRolesRepository.Table on userRole.RoleId equals role.Id
                               where userRole.UserId == userId
                               select role).FirstOrDefault();
            return roleDetails;
         }

        public ErrorModel AddUserToRole(string userId, string roleName)
        {
            var response = new ErrorModel();

            var roleDetails = _aspNetRolesRepository.Table.Where(x => x.Name == roleName).FirstOrDefault();
            if (roleDetails == null)
            {
                response.HasError = true;
                response.ErrorMessage = "Invalid role " + roleName;
                return response;
            }

            var existingRole = _aspNetUserRolesRepository.Table.Where(x => x.UserId == userId && x.RoleId == roleDetails.Id).FirstOrDefault();
            if (existingRole == null)
            {
                var newUserRole = new AspNetUserRoles();
                newUserRole.UserId = userId;
                newUserRole.RoleId = roleDetails.Id;
                _aspNetUserRolesRepository.Insert(newUserRole);
            }
                
            return response;

        }
    }
}
