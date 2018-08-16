using Microsoft.AspNetCore.Identity.MongoDB;

namespace WebApi.Authentication
{
    public class AppUser : IdentityUser
    {
        //stored as int value
        public AppRole UserRole { get; set; } = AppRole.StandartUser;
    }
}
