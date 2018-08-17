using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace WebApi.Authentication
{
    public class AppUser : MongoIdentityUser<Guid>
    {
    }
}
