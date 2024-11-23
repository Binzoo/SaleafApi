using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using SeleafAPI.Data;

namespace SeleafAPI.Models;


public class StudentMarksUpload
{
    public int Id { get; set; }
    public string FileUrl { get; set; }
    [Required] 
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}