using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class ManualPaymentDocDTO
    {
        [Required]
        public int ReferenceNumber { get; set; }
        [Required]
        public IFormFile DocFile { get; set; }
        [Required]
        public double Amount { get; set; }
    }
}