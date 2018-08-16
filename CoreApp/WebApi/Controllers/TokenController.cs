﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Authentication;
using WebApi.ViewModels;

namespace WebApi.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenGenerator _tokenGenerator;

        /// <summary>
        /// The manager for handling user creation, deletion, searching, roles etc...
        /// </summary>
        private readonly UserManager<AppUser> _userManager;
        
        public TokenController(
            IJwtTokenGenerator tokenGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            var appUser = new AppUser
            {
                UserName = "userName",
                Email = loginViewModel.Email,
                Id = Guid.NewGuid().ToString("N")
            };
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