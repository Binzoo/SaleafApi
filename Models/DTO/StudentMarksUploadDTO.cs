using System.ComponentModel.DataAnnotations;
using SeleafAPI.Data;

namespace SeleafAPI.Models.DTO;

public class StudentMarksUploadDTO
{
    public IFormFile File { get; set; }
    [Required] 
    public string Name { get; set; }
    public string Type { get; set; }
}