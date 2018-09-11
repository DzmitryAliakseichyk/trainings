using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}