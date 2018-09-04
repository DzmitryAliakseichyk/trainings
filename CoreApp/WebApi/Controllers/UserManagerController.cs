using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AdministratorOnly")]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailSender _emailSender;

        public UserManagerController(
            UserManager<AppUser> userManager,
            IPasswordGenerator passwordGenerator, 
            ITokenProvider tokenProvider, 
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
            _tokenProvider = tokenProvider;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <response code="200">User created succesfully</response>
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var password = _passwordGenerator.Generate();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ConfirmEmail", 
                    "Account",
                    values: new { userId = user.Id, code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                
                //todo: return created user
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Generate random user
        /// </summary>
        /// <remarks>
        /// This API used for testing
        /// </remarks>
        /// <response code="200">User created succesfully</response>
        [HttpGet]
        public async Task<IActionResult> CreateRandomUser()
        {
            var email = $"{Guid.NewGuid()}@test.com";
            var user = new AppUser { UserName = email, Email = email };
            var password = _passwordGenerator.Generate();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Revoke user. User will be sign out from all devices.
        /// </summary>
        /// <response code="200">Tokens are unregister</response>
        [HttpPost]
        public IActionResult RevokeUser([FromBody] Guid userId)
        {
            _tokenProvider.DeleteAccessTokenByUserId(userId);
            _tokenProvider.DeleteRefreshTokensByUserId(userId);
            return Ok();
        }
    }
}