using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationDownloadInput
    {
        public int Id { get; set; }
        public int ApplicationNumber { get; set; }
        public string DownloadMode { get; set; }  // Excel, Proposal
    }

    public class ApplicationDownloadResponse : DownloadResponse
    {        
        public int ApplicationNumber { get; set; }
        public int Id { get; set; }
    }
}
