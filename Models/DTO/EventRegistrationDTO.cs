using System.ComponentModel.DataAnnotations;

namespace SeleafAPI.Models.DTO;

public class EventRegistrationDTO
{
    [Required]
    public string FristName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required] public int NumberOfParticipant { get; set; }
    [Required]
    public int EventId { get; set; }
    
    [Required]
    public string PackageName { get; set; }

    [Required]
    public string CancelUrl { get; set; }  
    
    [Required]
    public string SuccessUrl { get; set; } 
    
    [Required]
    public string FailureUrl { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    public double Amount { get; set; }
    
    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; }
}