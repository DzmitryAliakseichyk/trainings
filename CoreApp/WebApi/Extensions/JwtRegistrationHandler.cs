using System;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApi.Authentication.Helpers;

namespace WebApi.Extensions
{
    public class JwtRegistrationHandler : IAuthorizationHandler
    {
        private readonly IJwtTokenHelper _jwtTokenHelper;
        private readonly ITokenProvider _tokenProvider;

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<JwtRegistrationHandler> _logger;

        public JwtRegistrationHandler(IJwtTokenHelper jwtTokenHelper, 
            ITokenProvider tokenProvider, 
            IHttpContextAccessor contextAccessor,
            ILogger<JwtRegistrationHandler> logger)
        {
            _jwtTokenHelper = jwtTokenHelper;
            _tokenProvider = tokenProvider;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }
        
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var httpContext = _contextAccessor.HttpContext;
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                var tokenStr = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var signature = _jwtTokenHelper.GetSignature(tokenStr);
                    var isRegistered = await _tokenProvider.IsAccessTokenRegistered(signature);
                    if (!isRegistered)
                    {
                        context.Fail();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    context.Fail();
                }
            }
        }
    }
}