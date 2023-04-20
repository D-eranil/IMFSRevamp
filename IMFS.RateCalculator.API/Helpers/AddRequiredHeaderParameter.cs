using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.RateCalculator.API.Helpers
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "CountryCode",
                In = ParameterLocation.Header,
                Description = "Country Code AU or NZ",
                Required = true,                 
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            }); ;
        }
    }
}
