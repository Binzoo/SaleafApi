using System;

namespace SeleafAPI.Models;

public class PropertyDetails
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? PhysicalAddress { get; set; }
    public string? ErfNoTownship { get; set; }
    public DateTime DatePurchased { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal MunicipalValue { get; set; }
    public decimal PresentValue { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
