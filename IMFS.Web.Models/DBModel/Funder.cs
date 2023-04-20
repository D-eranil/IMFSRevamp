using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Funder")]
	public partial class Funder: BaseEntity
    {
		[Key]
		public int Id { get; set; }
		
		public string FunderCode { get; set; }
		
		public string FunderName { get; set; }
		
		public string API_URL { get; set; }
		
		public string ContactEmailAdd { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedDate { get; set; }
		public DateTime ModifiedDate { get; set; }
		public string QuoteDownloadTCs { get; set; }


	}
}
