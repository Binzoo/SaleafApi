using Microsoft.Build.Framework;

namespace SeleafAPI.Models.DTO
{
    public class UserRoleDTO
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
