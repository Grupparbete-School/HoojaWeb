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
    }
}
