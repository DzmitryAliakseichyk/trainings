using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.Extensions;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ITokenProvider _tokenProvider;

        public AccountController(
            UserManager<AppUser> userManager, 
            IEmailSender emailSender, 
            IPasswordGenerator passwordGenerator, 
            ITokenProvider tokenProvider
            )
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _passwordGenerator = passwordGenerator;
            _tokenProvider = tokenProvider;
        }

        /// <summary>
        /// Confirm user email
        /// </summary>
        /// <response code="200">User email confirmed</response>
        /// <response code="400">Input parameters are wrong</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmationViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, model.Token);

            if (result.Succeeded)
            {
                //todo: move email text and subject to config
                await _emailSender.SendEmailAsync(user.Email, "Email was confirmed",
                    "Your email was confirmed");
                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Send confirmation to new user email
        /// </summary>
        /// <response code="200">Send confirmation email</response>
        /// <response code="400">Input parameters are wrong</response>
        /// <response code="403">Current user is not equals to updated user</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailViewModel model)
        {
            var userId = User.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

            //todo: generate url link to ui route, that will handle email changing
             var callbackUrl = $"{Request.Scheme}:\\\\{Request.Host}\\UI_ROUTE?email={model.NewEmail}&token={token}";

            //todo: move email text and subject to config
            await _emailSender.SendEmailAsync(model.NewEmail, "Update Email",
                $"Please confirm your new email by clicking here: <a href=\"{callbackUrl}\">link</a>");

            return Ok();
        }

        /// <summary>
        /// Update user email
        /// </summary>
        /// <response code="200">User email updated</response>
        /// <response code="400">Input parameters are wrong</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateNewEmail([FromBody] EmailConfirmationViewModel model)
        {
            var userId = User.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangeEmailAsync(user, model.Email, model.Token);

            if (result.Succeeded)
            {
                //todo: move email text and subject to config
                await _emailSender.SendEmailAsync(user.Email, "Email was updated",
                    "Your email was updated");
                
                _tokenProvider.DeleteAccessTokenByUserId(user.Id);
                _tokenProvider.DeleteRefreshTokensByUserId(user.Id);

                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <response code="200">User email confirmed</response>
        /// <response code="404">User not found</response>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            var userId = User.GetUserId();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }
            
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            
            //todo: validate new password
            var result = await _userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);

            if (result.Succeeded)
            {
                //todo: move email text and subject to config
                await _emailSender.SendEmailAsync(user.Email, "Password was updated",
                    "Your password was updated");

                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }
    }
}