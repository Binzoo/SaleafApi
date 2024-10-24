using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class PropertyDetailsDto
    {
        public string? PhysicalAddress { get; set; }
        public string? ErfNoTownship { get; set; }
        public DateTime DatePurchased { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal MunicipalValue { get; set; }
        public decimal PresentValue { get; set; }
    }
}