using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}