using System.Collections.Generic;

namespace SeleafAPI.Models;

public class Event
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public string? Location { get; set; }
    public string? EventImageUrl { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Status { get; set; }
    public bool Publish { get; set; }
    public int? Capacity { get; set; }

    public ICollection<Package> Packages { get; set; } = new List<Package>();
}
