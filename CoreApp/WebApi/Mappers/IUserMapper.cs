using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Mappers
{
    public interface IUserMapper
    {
        UserViewModel Map(AppUser appUser);
    }
}