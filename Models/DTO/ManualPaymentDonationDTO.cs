using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace SeleafAPI.Models.DTO
{
    public class ManualPaymentDonationDTO
    {
        [Required(ErrorMessage = "Amount is required")]
        public double Amount { get; set; }
        public string Currency { get; set; } = "ZAR";
        public bool isAnonymous { get; set; } = false;
    }
}