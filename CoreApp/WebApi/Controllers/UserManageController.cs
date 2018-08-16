using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdministratorOnly")]
    public class UserManageController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordGenerator _passwordGenerator;

        public UserManageController(UserManager<AppUser> userManager,
            IPasswordGenerator passwordGenerator)
        {
            _userManager = userManager;
            _passwordGenerator = passwordGenerator;
        }


        [HttpPost]
        public async Task<IActionResult> Post(CreateUserViewModel model)
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

        //Generate random user
        [HttpGet]
        public async Task<IActionResult> Get()
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
    }
}