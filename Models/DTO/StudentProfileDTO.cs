using Microsoft.Build.Framework;

namespace SeleafAPI.Models.DTO;

public class StudentProfileDTO
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public List<string>? Skills { get; set; } 
    [Required]
    public List<string>? Achievements { get; set; }

    [Required]
    public string? Year { get; set; }
    [Required]
    public bool IsFinalYear { get; set; }
    [Required]
    public string? Bio { get; set; }

    [Required] 
    public DateTime? GraduationDate { get; set; }
    [Required]
    public string? University { get; set; }

    [Required]
    public string? Degree { get; set; }
    [Required]
    public string? OnlineProfile { get; set; }

    [Required]
    public IFormFile? ProfileImage { get; set; } 
}