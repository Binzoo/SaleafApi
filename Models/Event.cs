using System;

namespace SeleafAPI.Models;

public class Event
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public bool Publish { get; set; }
}