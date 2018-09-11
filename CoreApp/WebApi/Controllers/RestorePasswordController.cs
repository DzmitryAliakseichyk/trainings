using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.Email;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class RestorePasswordController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailTemplateProvider _emailTemplateProvider;

        public RestorePasswordController(
            UserManager<AppUser> userManager, 
            IEmailSender emailSender, 
            IPasswordGenerator passwordGenerator, 
            IEmailTemplateProvider emailTemplateProvider)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _passwordGenerator = passwordGenerator;
            _emailTemplateProvider = emailTemplateProvider;
        }

        /// <summary>
        /// Send email with callback link thats reset password
        /// </summary>
        /// <response code="200">Email was sent</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        public async Task<IActionResult> SendPasswordResetToken([FromBody] ForgotPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //todo: generate url to ui that will handle reset password
            var callbackUrl = $"{Request.Scheme}:\\\\{Request.Host}\\UI_ROUTE?email={user.Email}&token={token}";

            //todo: move email text and subject to config
            await _emailSender.SendEmailAsync(model.Email, _emailTemplateProvider.GetSubject(EmailTemplateNames.ForgotPassword),
                _emailTemplateProvider.GetEmailBody(EmailTemplateNames.ForgotPassword, new[]
                {
                    HtmlEncoder.Default.Encode(callbackUrl)
                }));
            return Ok();
        }

        /// <summary>
        /// Send email with new password
        /// </summary>
        /// <response code="200">Email was sent</response>
        /// <response code="400">UserId or code is null</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            var password = _passwordGenerator.Generate();
            var result = await _userManager.ResetPasswordAsync(user, model.Token, password);
            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync(user.Email, _emailTemplateProvider.GetSubject(EmailTemplateNames.ForgotPassword),
                    _emailTemplateProvider.GetEmailBody(EmailTemplateNames.ForgotPassword, new[]
                    {
                        password
                    }));

                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }
    }
}