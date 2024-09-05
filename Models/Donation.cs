using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SeleafAPI.Data;

namespace SeleafAPI.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        public double Amount { get; set; }

        public string Currency { get; set; } = "ZAR";

        public string? PaymentId { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool isAnonymous { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
