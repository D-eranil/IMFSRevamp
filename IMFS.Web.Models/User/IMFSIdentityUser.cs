using Microsoft.AspNetCore.Identity;

namespace IMFS.Web.Models.User
{
    public class IMFSIdentityUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public bool Active { get; set; }
        public int DefaultFinanceType { get; set; }
        public int DefaultFinanceDuration { get; set; }
        public string DefaultFinanceFrequency { get; set; }
        public bool ExcludeGST { get; set; }
    }
}
