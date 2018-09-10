using System;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Authentication.Models
{
    public class AppUser : IdentityUser<Guid>
    {
    }
}
