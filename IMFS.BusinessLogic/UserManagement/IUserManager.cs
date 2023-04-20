using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using IMFS.Web.Models.User;
using System.Collections.Generic;

namespace IMFS.BusinessLogic.UserManagement
{
    public interface IUserManager
    {
        List<UserModel> GetUsers();

        List<UserModel> SearchUser(UserSearchModel userSearchModel);

        ErrorModel CreateNewUser(IMFSIdentityUser identityUser);

        ErrorModel SendNewUserEmail(string templateFolderPath, IMFSIdentityUser newUser);

        AspNetUsers GetUserDetails(string userId);

        AspNetUsers GetUserDetailsByEmail(string email);

        AspNetUsers GetUserDetailsByUserName(string username);

        ErrorModel UpdateUserStatus(UserModel user);

        ErrorModel DeactivateUserStatus(string userId);

        ErrorModel ActivateUserStatus(string userId);


    }
}
