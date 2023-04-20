using IMFS.Web.Models.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Application
{
    public  class ApplicationUpdateResponseModel:ErrorModel
    {
        public int ApplicationNumber { get; set; }
        public ApplicationUpdateResponseModel()
        {

        }
    }
}
