using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IMFS.Web.Api.Helper
{
    public class OktaTokenHandler : JwtSecurityTokenHandler
    {
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // base.ValidateToken will throw if the token is invalid
            // in any way (according to validationParameters)
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);
            var jwtToken = ReadJwtToken(token);
            if (jwtToken.Header?.Alg == null || jwtToken.Header?.Alg != SecurityAlgorithms.RsaSha256)
            {
                throw new SecurityTokenValidationException("The JWT token's signing algorithm must be RS256.");
            }
            return claimsPrincipal;
        }
    }
}
