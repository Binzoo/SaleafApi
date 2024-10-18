using System;
using System.ComponentModel.DataAnnotations;

namespace SeleafAPI.Models.DTO;

public class DirectorDTO
{
    [Required]
    public string? DirectorName { get; set; }
    [Required]
    public string? DirectorLastName { get; set; }
    [Required]
    public IFormFile DirectorImage { get; set; }
    [Required]
    public string? DirectorDescription { get; set; }
}
