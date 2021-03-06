﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.Email;
using WebApi.Mappers;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdministratorOnly")]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IUserMapper _mapper;
        private readonly ILogger<UserManagerController> _logger;

        public UserManagerController(
            UserManager<AppUser> userManager,
            IPasswordGenerator passwordGenerator, 
            ITokenProvider tokenProvider, 
            IEmailSender emailSender,
            IUserMapper mapper,
            ILogger<UserManagerController> logger, 
            IEmailTemplateProvider emailTemplateProvider)
        {
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
            _tokenProvider = tokenProvider;
            _emailSender = emailSender;
            _logger = logger;
            _emailTemplateProvider = emailTemplateProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <response code="200">List of all users</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserViewModel>), 200)]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userManager
                    .Users
                    .AsEnumerable()
                    .Select(_mapper.Map)
                    .ToList();

                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Get all user
        /// </summary>
        /// <response code="200">Requested user</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map(user));
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <response code="201">User created succesfully</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserViewModel), 201)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel model)
        {
            //todo: check if user exist
            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var password = _passwordGenerator.Generate();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //todo: generate url link to ui route, that will handle email confirmation
                var callbackUrl = $"{Request.Scheme}:\\\\{Request.Host}\\UI_ROUTE?email={user.Email}&token={token}";

                await _emailSender.SendEmailAsync(
                    user.Email, 
                    _emailTemplateProvider.GetSubject(EmailTemplateNames.ConfirmEmail),
                    _emailTemplateProvider.GetEmailBody(EmailTemplateNames.ConfirmEmail, new string[] { HtmlEncoder.Default.Encode(callbackUrl), password}));
                
                //todo: return valid Uri
                return Created(string.Empty, _mapper.Map(user));
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Lock user. User will be sign out from all devices.
        /// </summary>
        /// <response code="200">User is blocked</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> BlockUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                _tokenProvider.DeleteAccessTokenByUserId(userId);
                _tokenProvider.DeleteRefreshTokensByUserId(userId);


                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <response code="200">User is restored</response>
        /// <response code="404">User not found</response>
        [HttpPost("{userId}/_restore")]
        public async Task<IActionResult> UnlockUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, result.Errors);
        }

        /// <summary>
        /// Revoke user. User will be sign out from all devices.
        /// </summary>
        /// <response code="200">Tokens are unregister</response>
        [HttpDelete("{userId}/_kick")]
        public IActionResult KickUser(Guid userId)
        {
            try
            {
                _tokenProvider.DeleteAccessTokenByUserId(userId);
                _tokenProvider.DeleteRefreshTokensByUserId(userId);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}