using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.Extensions
{
    public class SecureHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecureHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            IHeaderDictionary headers = context.Response.Headers;

            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-XSS-Protection"] = "1; mode=block";

            await _next(context);
        }
    }
}
