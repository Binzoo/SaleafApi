using System;
using System.ComponentModel.DataAnnotations;

namespace SeleafAPI.Models.DTO;

public class EventDTO
{
    [Required(ErrorMessage = "Event Name is required")]
    public string? EventName { get; set; }
    [Required(ErrorMessage = "Event Description is required")]
    public string? EventDescription { get; set; }
    [Required(ErrorMessage = "Start Date is required")]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "End Date is required")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Publish is required")]
    public bool Publish { get; set; }
}