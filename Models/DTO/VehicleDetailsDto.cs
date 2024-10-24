using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class VehicleDetailsDto
    {
        public string? MakeModelYear { get; set; }
        public string? RegistrationNumber { get; set; }
        public decimal PresentValue { get; set; }
    }
}