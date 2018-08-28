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
using WebApi.Authentication.Generators;
using WebApi.Authentication.Helpers;
using WebApi.Authentication.Models;
using WebApi.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly ITokenProvider _tokenProvider;

        private readonly IJwtTokenHelper _jwtTokenHelper;

        public AuthController(IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            IRefreshTokenRepository refreshTokenRepository, 
            ITokenProvider tokenProvider, 
            IJwtTokenHelper jwtTokenHelper)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenProvider = tokenProvider;
            _jwtTokenHelper = jwtTokenHelper;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        /// <summary>
        /// SignIn into application.
        /// </summary>
        /// <response code="200">Tokens are generated and registered</response>
        /// <response code="403">User is locked or not allowed</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal error (tokens are not generated or registered)</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JwtTokenViewModel), 200)]
        public async Task<IActionResult> SignIn([FromBody] LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null)
            {
                return new NotFoundResult();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            if (result.Succeeded)
            {
                var token = new JwtTokenViewModel
                {
                    AccessToken = _jwtTokenGenerator.Generate(user),
                    RefreshToken = _refreshTokenGenerator.Generate()
                };

                try
                {
                    await _tokenProvider.RegisterRefreshToken(token.RefreshToken, user.Id);
                    await _tokenProvider.RegisterAccessToken(
                        _jwtTokenHelper.GetSignature(token.AccessToken),
                        _jwtTokenHelper.GetExpirationDate(token.AccessToken),
                        user.Id);
                }
                catch (Exception)
                {
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }
                
                return new OkObjectResult(token);
            }

            if (result.IsLockedOut || result.IsNotAllowed)
            {
                return new ForbidResult();
            }

            return new ForbidResult();
        }

        /// <summary>
        /// SignOut from application.
        /// </summary>
        /// <response code="200">Tokens are unregistered</response>
        /// <response code="500">Internal error (tokens are not unregister)</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SignOut([FromBody] SignOutViewModel signOutViewModel)
        {
            //todo: check is user exist

            try
            {
                await _tokenProvider.DeleteRefreshTokenById(signOutViewModel.RefreshToken);
                await _tokenProvider.DeleteAccessToken(signOutViewModel.AccessTokenSignature);
            }
            catch (Exception)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            return new OkResult();
        }
    }
}