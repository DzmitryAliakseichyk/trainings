using WebApi.Models;

namespace WebApi.Authentication
{
    public interface IJwtTokenGenerator
    {
        string Generate(AppUser user);
    }
}