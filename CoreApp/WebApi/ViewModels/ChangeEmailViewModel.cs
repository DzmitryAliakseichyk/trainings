using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string NewEmail { get; set; }
    }
}