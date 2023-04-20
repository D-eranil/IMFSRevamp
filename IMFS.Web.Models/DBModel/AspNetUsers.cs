using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("AspNetUsers")]
    public partial class AspNetUsers : BaseEntity
    {
        [Key]
        public string Id { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public int? DefaultFinanceType { get; set; }
        public int? DefaultFinanceDuration { get; set; }
        public string DefaultFinanceFrequency { get; set; }

        public bool Active { get; set; }
        public bool ExcludeGST { get; set; }

    }
}
