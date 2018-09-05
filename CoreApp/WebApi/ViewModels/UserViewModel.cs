using System;

namespace WebApi.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsLocked { get; set; }
    }
}