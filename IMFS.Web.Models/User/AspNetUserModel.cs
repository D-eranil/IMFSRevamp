using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.User
{
    public class AspNetUserModel
    {
        
        public string Id { get; set; }

        public string Title { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public int? DefaultFinanceType { get; set; }
        public int? DefaultFinanceDuration { get; set; }
        public string DefaultFinanceFrequency { get; set; }
        public string DefaultFinanceTypeName { get; set; }
        public string DefaultFinanceDurationName { get; set; }
        //Editable by IMStaff
        public bool Active { get; set; }
        public bool ExcludeGST { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }

    }
}
