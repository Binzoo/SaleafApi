using System.ComponentModel.DataAnnotations;
namespace SeleafAPI.Models.DTO;

public class EventRegistrationDTO
{
    [Required]
    public int EventId { get; set; }
    [Required]
    public string CancelUrl { get; set; }  
    
    [Required]
    public string SuccessUrl { get; set; } 
    [Required]
    public string FailureUrl { get; set; }
    [Required(ErrorMessage = "Amount is required")]
    public double Amount { get; set; }
    [Required(ErrorMessage = "Currency is required")]
    public string  Currecny { get; set; }

}