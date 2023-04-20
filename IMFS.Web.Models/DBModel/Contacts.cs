using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMFS.Web.Models.DBModel
{
    [Table("Contacts")]
    public class Contacts : BaseEntity
    {
        [Key]
        public string ContactID { get; set; }
        public string ContactEmail { get; set; }
        public string ResellerID { get; set; }
        public int ContactType { get; set; }
        public string ContactName { get; set; }
        public DateTime? ContactDOB { get; set; }
        public string ContactAddress { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactABNACN { get; set; }
        public string ContactPosition { get; set; }
        public bool IsContactSignatory { get; set; }
        public string ContactPhone { get; set; }
    }

    public class TempContact : BaseEntity
    {
        [Key]
        public string ContactID { get; set; }
        public string ContactEmail { get; set; }
        public string ResellerID { get; set; }
        public int ContactType { get; set; }
        public string ContactName { get; set; }
        public string ContactDOB { get; set; }
        public string ContactAddress { get; set; }
        public string ContactDriversLicNo { get; set; }
        public string ContactABNACN { get; set; }
        public string ContactPosition { get; set; }
        public bool IsContactSignatory { get; set; }
        public string ContactPhone { get; set; }
    }
}
