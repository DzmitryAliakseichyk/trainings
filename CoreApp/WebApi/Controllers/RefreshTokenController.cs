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

        /// <summary>
        /// Get new access Token
        /// </summary>
        /// <response code="200">Tokens are generated and registered</response>
        /// <response code="404">Invalid refresh token</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal error (tokens are not generated or registered)</response>
        [HttpGet("{refreshToken}")]
        [ProducesResponseType(typeof(JwtTokenViewModel), 200)]
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

            //todo: check is user locked

            var token = new JwtTokenViewModel
            {
                AccessToken = _tokenGenerator.Generate(user),
                RefreshToken = refreshToken.ToString()
            };

            try
            {
                await _tokenProvider.UpdateRefreshToken(refreshToken);

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
    }
}