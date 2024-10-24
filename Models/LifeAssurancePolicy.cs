using System;

namespace SeleafAPI.Models;

public class LifeAssurancePolicy
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? Company { get; set; }
    public string? Description { get; set; }
    public decimal SurrenderValue { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
