using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebApi.Authentication;
using WebApi.Authentication.Models;

namespace WebApi
{
    internal class UserDatabaseInitializer
    {
        internal static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppIdentityDbContext>();
                context.Database.EnsureCreated();

                var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<UserDatabaseInitializer>>();
                logger.LogInformation($"Start {nameof(UserDatabaseInitializer.Initialize)}");
                var task = Task.Factory.StartNew(async () =>
                {
                    var roles = Enum.GetValues(typeof(AppRoleEnum)).Cast<AppRoleEnum>();
                    foreach (var role in roles)
                    {

                        var roleExists = await roleManager.RoleExistsAsync(role.ToString());
                        if (!roleExists)
                        {
                            await roleManager.CreateAsync(new AppRole
                            {
                                RoleEnumValue = role,
                                Name = role.ToString()
                            });
                        }
                    }

                    var superAdmin = await userManager.FindByNameAsync(configuration["SuperAdmin:Name"]);

                    if (superAdmin == null)
                    {
                        superAdmin = new AppUser
                        {
                            UserName = configuration["SuperAdmin:Name"],
                            Email = configuration["SuperAdmin:Email"],
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(superAdmin, configuration["SuperAdmin:Password"]);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRolesAsync(superAdmin,
                                new[]
                                {
                                    AppRoleEnum.SuperAdministrator.ToString(),
                                    AppRoleEnum.Administrator.ToString()
                                });
                        }

                    }
                });

                try
                {
                    task.Unwrap().Wait();
                    logger.LogInformation($"Finish {nameof(UserDatabaseInitializer.Initialize)}");
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);

                    var aggrEx = task.Exception;
                    foreach (var exception in aggrEx.InnerExceptions)
                    {
                        logger.LogError(exception, exception.Message);
                    }
                    throw;
                }
               


            }
        }
    }
}
