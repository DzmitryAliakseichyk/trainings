using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordGenerator _passwordGenerator;

        public AccountController(
            UserManager<AppUser> userManager, 
            IEmailSender emailSender, 
            IPasswordGenerator passwordGenerator
            )
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _passwordGenerator = passwordGenerator;
        }

        /// <summary>
        /// Confirm user email
        /// </summary>
        /// <response code="200">User email confirmed</response>
        /// <response code="400">UserId or code is null</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);

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
        /// <response code="400">UserId or code is null</response>
        /// <response code="403">Current user is not equals to updated user</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailViewModel viewModel)
        {
            var userId = viewModel.UserId;

            if (userId == null)
            {
                return BadRequest();
            }

            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Sid))?.Value;

            if (string.IsNullOrWhiteSpace(currentUserId) || !currentUserId.Equals(userId))
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var code = await _userManager.GenerateChangeEmailTokenAsync(user, viewModel.NewEmail);

            var callbackUrl = Url.Action("UpdateNewEmail", "Account", new { userId = user.Id, code = code, newEmail = viewModel.NewEmail }, protocol: HttpContext.Request.Scheme);

            //todo: move email text and subject to config
            await _emailSender.SendEmailAsync(viewModel.NewEmail, "Update Email",
                $"Please confirm your new email by clicking here: <a href=\"{callbackUrl}\">link</a>");

            return Ok();
        }

        /// <summary>
        /// Update user email
        /// </summary>
        /// <response code="200">User email updated</response>
        /// <response code="400">UserId or code is null</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateNewEmail(string userId, string code, string newEmail)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangeEmailAsync(user, newEmail, code);

            if (result.Succeeded)
            {
                //todo: move email text and subject to config
                await _emailSender.SendEmailAsync(user.Email, "Email was updated",
                    "Your email was updated");
                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Change user's password
        /// </summary>
        /// <response code="200">User email confirmed</response>
        /// <response code="400">UserId or code is null</response>
        /// <response code="404">User not found</response>
        [HttpPatch("{userId}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId,
            [FromBody] ChangePasswordViewModel viewModel)
        {
            if (userId == null)
            {
                return BadRequest();
            }

            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Sid))?.Value;

            if (string.IsNullOrWhiteSpace(currentUserId) || !currentUserId.Equals(userId))
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);

            if (result.Succeeded)
            {
                //todo: move email text and subject to config
                await _emailSender.SendEmailAsync(user.Email, "Password was updated",
                    "Your password was updated");

                return Ok();
            }

            //todo: check if password is weak and etc.
            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }
    }
}