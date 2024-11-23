using System.ComponentModel.DataAnnotations;
using SeleafAPI.Data;

namespace SeleafAPI.Models.DTO;

public class StudentMarksUploadDTO
{
    public int Id { get; set; }
    public IFormFile File { get; set; }
    [Required] 
    public string Name { get; set; }
    public string Type { get; set; }
}