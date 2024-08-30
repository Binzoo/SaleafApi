using System.ComponentModel.DataAnnotations;

namespace SeleafAPI.Models.DTO
{
    public class PasswordResetDTO
    {
        public int Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? Code { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
