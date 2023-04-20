using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.RateCalculator.API.Helpers
{
    public static class LogRequestAndResponseMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestAndResponseMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogRequestAndResponseMiddleware>();
        }
    }
}
