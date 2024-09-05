using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeleafAPI.Data;

namespace SaleafApi.Models
{
    public class DonorCertificateInfo
    {
        public int Id { get; set; }
        public string? IdentityNoOrCompanyRegNo { get; set; }
        public string? IncomeTaxNumber { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}