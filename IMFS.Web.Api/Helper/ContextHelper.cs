using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IMFS.Web.Api.Helper
{
    public class ContextHelper
    {
        //private readonly IHttpContextAccessor accessor;


        //public ContextHelper(IHttpContextAccessor accessor)
        //{
        //    this.accessor = accessor;
        //}

        //public List<Claim> UserClaims
        //{
        //    get
        //    {
        //        return accessor?.HttpContext?.User.Claims;
        //    }
        //}

        public static string CountryCode { get; set; }

        public static string GetCountryCode()
        {
            return CountryCode;
        }

        //public string GetCurrentUserName()
        //{
        //    return UserClaims.FindFirst("FullName").Value;
        //}

        //public static string GetUserEmail()
        //{
        //    return UserClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        //}

        //public ClaimsPrincipal GetUser()
        //{
        //    return accessor?.HttpContext?.User;
        //}
    }
}
