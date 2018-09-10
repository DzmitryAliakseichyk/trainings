using System;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Authentication.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public AppRoleEnum RoleEnumValue { get; set; }
    }
}
