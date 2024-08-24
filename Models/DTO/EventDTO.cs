using System;

namespace SeleafAPI.Models.DTO;

public class EventDTO
{
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}