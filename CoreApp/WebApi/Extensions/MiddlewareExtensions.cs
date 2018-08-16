using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApi.Extensions
{
    internal static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSecureHeadersMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecureHeadersMiddleware>();
        }
    }
}
