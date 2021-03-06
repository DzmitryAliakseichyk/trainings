﻿using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.Email;
using WebApi.Extensions;
using WebApi.Mappers;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IUserMapper _mapper;

        public AccountController(
            UserManager<AppUser> userManager, 
            IEmailSender emailSender, 
            IPasswordGenerator passwordGenerator, 
            ITokenProvider tokenProvider, 
            IEmailTemplateProvider emailTemplateProvider, 
            IUserMapper mapper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _tokenProvider = tokenProvider;
            _emailTemplateProvider = emailTemplateProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Get user info
        /// </summary>
        /// <response code="200">Return user information</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public async Task<IActionResult> GetUser()
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

            return Ok(_mapper.Map(user));
        }

        /// <summary>
        /// Update user info
        /// </summary>
        /// <remarks>
        /// This action does nothing
        /// </remarks>
        /// <response code="200">User updated</response>
        /// <response code="403">Not authorized</response>
        /// <response code="404">User not found</response>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel model)
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

            //update user
            //user.UserName == model.UserName;

            //var result = await _userManager.UpdateAsync(user);

            //if (result.Succeeded)
            //{
            //    return Ok();
            //}

            //return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
            
            return Ok();
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
                await _emailSender.SendEmailAsync(user.Email, _emailTemplateProvider.GetSubject(EmailTemplateNames.EmailConfirmed),
                    _emailTemplateProvider.GetEmailBody(EmailTemplateNames.EmailConfirmed));
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
        public async Task<IActionResult> SendChangeEmailToken([FromBody] ChangeEmailViewModel model)
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

            await _emailSender.SendEmailAsync(model.NewEmail, _emailTemplateProvider.GetSubject(EmailTemplateNames.UpdateEmail),
                _emailTemplateProvider.GetEmailBody(EmailTemplateNames.EmailConfirmed, new[]
                {
                    HtmlEncoder.Default.Encode(callbackUrl)
                }));

            return Ok();
        }

        /// <summary>
        /// Update user email
        /// </summary>
        /// <response code="200">User email updated</response>
        /// <response code="400">Input parameters are wrong</response>
        /// <response code="404">User not found</response>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody] EmailConfirmationViewModel model)
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
                await _emailSender.SendEmailAsync(user.Email, _emailTemplateProvider.GetSubject(EmailTemplateNames.EmailUpdated),
                    _emailTemplateProvider.GetEmailBody(EmailTemplateNames.EmailUpdated));
                
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
                await _emailSender.SendEmailAsync(user.Email, _emailTemplateProvider.GetSubject(EmailTemplateNames.PasswordUpdated),
                    _emailTemplateProvider.GetEmailBody(EmailTemplateNames.PasswordUpdated));

                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }
    }
}