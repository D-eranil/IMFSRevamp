using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IMFS.Web.Models.DBModel
{
    [Table("FinanceType")]
    public partial class FinanceType: BaseEntity
    {
        public int Id { get; set; }
        public int QuoteDurationType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }       
        public DateTime? ModifiedDate { get; set; }       
        public DateTime? CreatedDate { get; set; }

    }
}
