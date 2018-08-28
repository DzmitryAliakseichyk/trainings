using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace WebApi.Authentication.Models
{
    public class AppUser : MongoIdentityUser<Guid>
    {
    }
}
