using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class DependentInfoDto
    {
        public string? FullName { get; set; }
        public string? RelationshipToApplicant { get; set; }
        public int Age { get; set; }
        public string? InstitutionName { get; set; }
    }
}