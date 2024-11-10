using SeleafAPI.Data;

namespace SeleafAPI.Models;

public class EventRegistration
{
    public int Id { get; set; }  
        
    // Foreign key for Event
    public int EventId { get; set; }
    public Event? Event { get; set; }  

    // Foreign key for User
    public string? UserId { get; set; }
    public AppUser? User { get; set; }  
    
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;  
}