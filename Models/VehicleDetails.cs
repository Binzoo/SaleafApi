using System;

namespace SeleafAPI.Models;

public class VehicleDetails
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? MakeModelYear { get; set; }
    public string? RegistrationNumber { get; set; }
    public decimal PresentValue { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
