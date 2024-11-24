namespace SeleafAPI.Models.DTO;

public class EventWithPackagesDTO
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public string? EventDescription { get; set; }
    public string? Location { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public bool Publish { get; set; }
    public string? EventImageUrl { get; set; }
    public string? Status { get; set; }
    
    public int? Capacity { get; set; }
    public List<PackageDTO> Packages { get; set; } = new();
}