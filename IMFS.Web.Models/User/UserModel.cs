using IMFS.Web.Models.Misc;
using System.Collections.Generic;

namespace IMFS.Web.Models.User
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public int? DefaultFinanceType { get; set; }
        public int? DefaultFinanceDuration { get; set; }
        public string DefaultFinanceFrequency { get; set; }
        public string DefaultFinanceTypeName { get; set; }
        public int DefaultFinanceDurationName { get; set; }

        public string PhoneNumber { get; set; }

        public List<LookupModel> Roles { get; set; }


        //Editable by IMStaff
        public bool Active { get; set; }
        public bool ExcludeGST { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }


    }

    public class UserSearchModel
    {

        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string JobTitle { get; set; }
        public string Email { get; set; }
        public int? DefaultFinanceType { get; set; }
        public int? DefaultFinanceDuration { get; set; }
        public string DefaultFinanceFrequency { get; set; }
        public string DefaultFinanceTypeName { get; set; }
        public int DefaultFinanceDurationName { get; set; }

        public string PhoneNumber { get; set; }


        public bool Active { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }


    }

    public class AutoCreateUserTemplateModel
    {
        public string IngramLogoHTML { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string Role { get; set; }
        public string IMFSHostLink { get; set; }
        public string IMFSUserLink { get; set; }
    }

    public class CurrentUserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CustomerNumber { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}
