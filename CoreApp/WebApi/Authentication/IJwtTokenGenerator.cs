using WebApi.Models;

namespace WebApi.Authentication
{
    public interface IJwtTokenGenerator
    {
        string Generate(AppUser user);
        string GetTokenSignature(string jwtToken);
    }
}