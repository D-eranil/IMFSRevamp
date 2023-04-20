using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("FunderPlan")]
    public partial class FunderPlan: BaseEntity
    {
        [Key]
        public int PlanId { get; set; }

        public string PlanDescription { get; set; }
    }
}
