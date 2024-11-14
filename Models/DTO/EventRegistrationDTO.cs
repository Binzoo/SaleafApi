using Microsoft.Build.Framework;

namespace SeleafAPI.Models.DTO;

public class EventRegistrationDTO
{
    [Required]
    public int EventId { get; set; }
    public string? CancelUrl { get; set; }  
    public string? SuccessUrl { get; set; } 
    public string? FailureUrl { get; set; }  
}