using System;

namespace SeleafAPI.Models;

public class OtherLiability
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? Description { get; set; }
    public decimal Amount { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
