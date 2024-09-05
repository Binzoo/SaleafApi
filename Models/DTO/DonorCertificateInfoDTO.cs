using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class DonorCertificateInfoDTO
    {
        [Required(ErrorMessage = "Identity number or Company Registration Number is required")]
        public string? IdentityNoOrCompanyRegNo { get; set; }
        [Required(ErrorMessage = "Income Task Number is required")]
        public string? IncomeTaxNumber { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string? PhoneNumber { get; set; }
    }
}