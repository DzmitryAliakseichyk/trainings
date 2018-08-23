using System;
using System.Net;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Helpers;
using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {

        private readonly IJwtTokenGenerator _tokenGenerator;

        private readonly UserManager<AppUser> _userManager;

        private readonly ITokenProvider _tokenProvider;

        private readonly IJwtTokenHelper _jwtTokenHelper;

        public RefreshTokenController(
            IJwtTokenGenerator tokenGenerator,
            UserManager<AppUser> userManager,
            ITokenProvider tokenProvider,
            IJwtTokenHelper jwtTokenHelper)
        {
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _jwtTokenHelper = jwtTokenHelper;
        }

        [HttpGet("{refreshToken}")]
        public async Task<IActionResult> RefreshAccessToken(Guid refreshToken)
        {
            var refreshTokenObject = await _tokenProvider.GetRefreshToken(refreshToken);

            if (refreshTokenObject == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(refreshTokenObject.UserId.ToString());

            if (user == null)
            {
                return BadRequest();
            }

            var token = new JwtTokenViewModel
            {
                AccessToken = _tokenGenerator.Generate(user),
                RefreshToken = refreshToken.ToString()
            };

            try
            {
                await _tokenProvider.UpdateRefreshToken(refreshToken);

                await _tokenProvider.CreateAccessToken(
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
    }
}