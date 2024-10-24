using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class FinancialDetailsDto
    {
        public string? Role { get; set; } // E.g., Father, Mother, Sibling, Applicant, Guardian
        public string? FullName { get; set; }
        public string? SAIDNumber { get; set; }
        public string? Occupation { get; set; }
        public string? MaritalStatus { get; set; }
        public decimal GrossMonthlyIncome { get; set; }
        public decimal OtherIncome { get; set; }
    }
}