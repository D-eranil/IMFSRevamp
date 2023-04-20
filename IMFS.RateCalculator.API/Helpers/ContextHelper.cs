using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.RateCalculator.API.Helpers
{
    public class ContextHelper
    {
        public static string CountryCode { get; set; }

        public static string GetCountryCode()
        {
            return CountryCode;            
        }

    }
}
