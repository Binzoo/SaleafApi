namespace SeleafAPI.Models;

public class Package
{
    public int PackageId { get; set; }
    public string PackageName { get; set; }
    public double PackagePrice { get; set; }
    
    public int EventId { get; set; }
    public Event Event { get; set; }
}