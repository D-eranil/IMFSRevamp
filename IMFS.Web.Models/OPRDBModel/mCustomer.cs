using IMFS.Web.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.OPRDBModel
{
    [Table("mCustomer")]
    public partial class mCustomer : BaseEntity
    {
        [Key]
        public Guid CustomerID { get; set; }

        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMasterName { get; set; }

        public string CustomerState { get; set; }
        public string Controller { get; set; }
        public string Parent { get; set; }
        public string ABN { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
