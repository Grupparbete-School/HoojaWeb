using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HoojaWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        //public string Email { get; set; }
        public string? SecurityNumber { get; set; }
        public string? PasswordHash { get; set; }
    }
}
