using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;

namespace ControleGastosPessoais.Helpers
{
    public static class JwtHelper
    {
        public static string GetValueJwtClaim(string jwt, string claimKey)
        {
            if (AuthenticationHeaderValue.TryParse(jwt, out var headerValue))
            {
                var parameter = headerValue.Parameter;
                var token = parameter;
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                return jwtSecurityToken.Claims.FirstOrDefault(xx => xx.Type == claimKey)!.Value;
            }

            return string.Empty;
        }
    }
}