using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Services.Models
{
    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }        
    }
}
