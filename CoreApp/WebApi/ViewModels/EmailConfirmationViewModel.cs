using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class EmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}