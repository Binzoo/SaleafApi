using System.Runtime.InteropServices.JavaScript;
using System.ComponentModel.DataAnnotations;


namespace SeleafAPI.Models.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last Name is required")]

        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        [Required]
        public bool isStudent { get; set; }
    }
}
