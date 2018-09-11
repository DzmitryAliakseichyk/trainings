using System.Linq;
using System.Security.Claims;

namespace WebApi.Extensions
{
    public static class UserExtentions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Sid))?.Value;
        }
    }
}