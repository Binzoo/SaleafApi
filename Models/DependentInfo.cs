using System;

namespace SeleafAPI.Models;

public class DependentInfo
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key

    public string? FullName { get; set; }
    public string? RelationshipToApplicant { get; set; }
    public int Age { get; set; }
    public string? InstitutionName { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
