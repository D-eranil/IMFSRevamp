using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Categories")]
    public partial class Categories: BaseEntity
    {
		public int Id { get; set; }

		public int? TypeID { get; set; }

		public DateTime LastSavedUtc { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string FinanceProductType { get; set; }
		public int FinanceProductTypeCode { get; set; }

		public bool? IsActive { get; set; }
		public bool? IsRental { get; set; }
		public bool? IsLease { get; set; }
		public bool? IsInstalment { get; set; }
	}
}
