using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace WebApi.Authentication.Models
{
    public class AppRole : MongoIdentityRole<Guid>
    {
        public AppRoleEnum RoleEnumValue { get; set; }
    }
}
