using System.ComponentModel.DataAnnotations;

public class PackageDTO
{
    
    [Required(ErrorMessage = "Package Name is required")]
    public string PackageName { get; set; }

    [Required(ErrorMessage = "Package Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Package Price must be a positive value")]
    public double PackagePrice { get; set; }

    public string? PackageDescription { get; set; }
}