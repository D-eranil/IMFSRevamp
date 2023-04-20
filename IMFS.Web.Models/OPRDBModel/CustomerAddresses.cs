using IMFS.Web.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.OPRDBModel
{
    [Table("CustomerAddresses")]
    public partial class CustomerAddresses : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string CustomerNumber { get; set; }
        public string AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

    }
}
