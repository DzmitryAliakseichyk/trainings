using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class SignOutViewModel
    {
        [Required]
        public Guid RefreshToken { get; set; }

        [Required]
        public string AccessTokenSignature { get; set; }
    }
}