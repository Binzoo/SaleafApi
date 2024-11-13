using Microsoft.Build.Framework;

namespace SeleafAPI.Models.DTO;

public class EventRegistrationDTO
{
    [Required]
    public int EventId { get; set; }
}