using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Mappers
{
    public class UserMapper : IUserMapper
    {
        public UserViewModel Map(AppUser appUser)
        {
            return new UserViewModel
            {
                Id = appUser.Id,
                Email = appUser.Email,
                UserName = appUser.UserName,
                IsEmailConfirmed = appUser.EmailConfirmed,
                IsLocked = appUser.LockoutEnd.HasValue
            };
        }
    }
}