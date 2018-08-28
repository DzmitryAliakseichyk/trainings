using WebApi.Authentication.Models;

namespace WebApi.Authentication.Generators
{
    public interface IJwtTokenGenerator
    {
        string Generate(AppUser user);
    }
}