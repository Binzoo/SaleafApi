using System;

namespace SeleafAPI.Models;

public class FinancialDetails
{
    public int Id { get; set; }
    public int BursaryApplicationId { get; set; } // Foreign Key
    public string? Role { get; set; } // E.g., Father, Mother, Sibling, Applicant, Guardian

    public string? FullName { get; set; }
    public string? SAIDNumber { get; set; }
    public string? Occupation { get; set; }
    public string? MaritalStatus { get; set; }
    public decimal GrossMonthlyIncome { get; set; }
    public decimal OtherIncome { get; set; }

    // Navigation Property
    public BursaryApplication BursaryApplication { get; set; }
}
