using System;

namespace SeleafAPI.Models;

public class InvestmentDetails
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? Company { get; set; }
    public string? Description { get; set; }
    public decimal MarketValue { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
