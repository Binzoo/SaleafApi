using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SeleafAPI.Models.DTO;

public class EventDTO
{
    [Required(ErrorMessage = "EventName is required")]
    public string? EventName { get; set; }

    [Required(ErrorMessage = "Event Description is required")]
    public string? EventDescription { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "StartDateTime is required")]
    [DataType(DataType.DateTime, ErrorMessage = "StartDateTime must be a valid date and time")]
    public DateTime StartDateTime { get; set; }

    [Required(ErrorMessage = "EndDateTime is required")]
    [DataType(DataType.DateTime, ErrorMessage = "EndDateTime must be a valid date and time")]
    public DateTime EndDateTime { get; set; }

    [Required(ErrorMessage = "Event Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "EventPrice must be a positive value")]
    public double EventPrice { get; set; }

    public bool Publish { get; set; }

    public IFormFile? EventImageFile { get; set; }
}