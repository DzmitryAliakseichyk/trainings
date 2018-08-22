using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Business.Providers;
using Common.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication;
using WebApi.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly ITokenProvider _tokenProvider;

        public AuthController(IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            IRefreshTokenRepository refreshTokenRepository, 
            ITokenProvider tokenProvider)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenProvider = tokenProvider;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SingIn([FromBody] LoginViewModel loginViewModel)
        {
            var appUser = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (appUser == null)
            {
                return new NotFoundResult();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginViewModel.Password, false);
            if (result.Succeeded)
            {
                var token = new JwtTokenViewModel
                {
                    AccessToken = _jwtTokenGenerator.Generate(appUser),
                    RefreshToken = _refreshTokenGenerator.Generate()
                };

                await _tokenProvider.Save(token.RefreshToken, _jwtTokenGenerator.GetTokenSignature(token.AccessToken), appUser.UserName);
                
                return new OkObjectResult(token);
            }

            if (result.IsLockedOut || result.IsNotAllowed)
            {
                return new ForbidResult();
            }

            return new ForbidResult();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SingOut([FromQuery] string refreshToken, [FromQuery] string accessTokenSignature)
        {
            await _tokenProvider.Delete(refreshToken, accessTokenSignature);
            return new OkResult();
        }
    }
}