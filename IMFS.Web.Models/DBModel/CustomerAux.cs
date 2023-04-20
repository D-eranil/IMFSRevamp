using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("CustomerAux")]
    
    public partial class CustomerAux : BaseEntity
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public bool? AccreditationStatus { get; set; }
        public string PartnerID { get; set; }
    }
}
