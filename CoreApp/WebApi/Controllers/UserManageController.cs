using System;
using System.Threading.Tasks;
using Business.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AdministratorOnly")]
    public class UserManageController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ITokenProvider _tokenProvider;

        public UserManageController(UserManager<AppUser> userManager,
            IPasswordGenerator passwordGenerator, 
            ITokenProvider tokenProvider)
        {
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
            _tokenProvider = tokenProvider;
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var password = _passwordGenerator.Generate();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

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

        [HttpPost]
        public async Task<IActionResult> RevokeUser([FromBody] Guid userId)
        {
            await _tokenProvider.DeleteAccessTokenByUserId(userId);
            await _tokenProvider.DeleteRefreshTokensByUserId(userId);
            return Ok();
        }
    }
}