using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public class ApplicationSaveResponseModel: ErrorModel
    {
        public int ApplicationId { get; set; }
        public int ApplicationNumber { get; set; }
        public ApplicationSaveResponseModel()
        {

        }
    }
}
