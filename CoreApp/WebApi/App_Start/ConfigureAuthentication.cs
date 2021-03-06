﻿using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi.Authentication.Models;
using WebApi.Extensions;

namespace WebApi
{
    internal class ConfigureAuthentication
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentity<AppUser, AppRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
                .AddMongoDbStores<AppUser, AppRole, Guid>(
                    configuration.GetSection("MongoConnection:ConnectionString").Value, 
                    configuration.GetSection("MongoConnection:Database").Value
                    )
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                    };
                });

            services.AddAuthorization(options =>
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole(
                    AppRoleEnum.Administrator.ToString(),
                    AppRoleEnum.SuperAdministrator.ToString())));

            services.AddSingleton<IAuthorizationHandler, JwtRegistrationHandler>();
        }
    }
}
