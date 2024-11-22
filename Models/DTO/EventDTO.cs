using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SeleafAPI.Models.DTO;

using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

public class EventDTO
{
    [Required(ErrorMessage = "EventName is required")]
    public string? EventName { get; set; }

    [Required(ErrorMessage = "Event Description is required")]
    public string? EventDescription { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "StartDateTime is required")]
    public DateTime StartDateTime { get; set; }

    [Required(ErrorMessage = "EndDateTime is required")]
    public DateTime EndDateTime { get; set; }

    public bool Publish { get; set; }

    [SwaggerSchema("Upload an image file for the event")]
    public IFormFile? EventImageFile { get; set; }

    [Required(ErrorMessage = "Packages are required")]
    public List<PackageDTO> Packages { get; set; } = new();
}
