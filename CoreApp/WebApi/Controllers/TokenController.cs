using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authentication.Generators;
using WebApi.Authentication.Models;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenGenerator _tokenGenerator;
        
        private readonly UserManager<AppUser> _userManager;

        public TokenController(IJwtTokenGenerator tokenGenerator,
            UserManager<AppUser> userManager)
        {
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAndLoginUser(LoginViewModel loginViewModel)
        {
            var appUser = new AppUser
            {
                UserName = "userName",
                Email = loginViewModel.Email
            };

            await _userManager.AddToRolesAsync(appUser, new List<string>
                {
                    AppRoleEnum.Administrator.ToString(),
                    AppRoleEnum.SuperAdministrator.ToString()
                }
            );

            return Ok(_tokenGenerator.Generate(appUser));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Check()
        {
            var user = HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                return Ok(user.Claims.Select(c=> new {c.Type , c.Value}));
            }
            return Ok("Anonimous");
        }
    }
}
