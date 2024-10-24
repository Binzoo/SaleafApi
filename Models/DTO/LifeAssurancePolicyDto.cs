using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class LifeAssurancePolicyDto
    {
        public string? Company { get; set; }
        public string? Description { get; set; }
        public decimal SurrenderValue { get; set; }
    }
}