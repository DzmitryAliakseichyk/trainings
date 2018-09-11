using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}