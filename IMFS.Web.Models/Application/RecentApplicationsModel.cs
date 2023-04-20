using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class RecentApplicationsModel
    {
        public int? Id { get; set; }
        public int? ApplicationNumber { get; set; }
        public String endUser { get; set; }
        public String Status { get; set; }
        public decimal? FinanaceAmount { get; set; }
        public string FinanceType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
