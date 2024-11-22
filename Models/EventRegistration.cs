using SeleafAPI.Data;

namespace SeleafAPI.Models;

public class EventRegistration
{
    public int Id { get; set; }  
    public string FristName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public int NumberOfParticipant { get; set; }
    // Foreign key for Event
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public string PacakageName { get; set; }    

    // Foreign key for User
    public string? UserId { get; set; }
    public AppUser? User { get; set; }  
    
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string Currency { get; set; }
    public double Amount { get; set; }
    public bool IsPaid { get; set; } = false;
    public string? PaymentId { get; set; }
}