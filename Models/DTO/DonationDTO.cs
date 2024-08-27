using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeleafAPI.Models.DTO
{
    public class DonationDTO
    {
        public int Amount { get; set; }  // Amount in cents (e.g., 1000 for R10)
        public string Currency { get; set; } = "ZAR";  // Default currency to ZAR
        public int DonationTypeId { get; set; }  // ID of the donation type
        public string? CancelUrl { get; set; }  // URL to redirect if the payment is canceled
        public string? SuccessUrl { get; set; }  // URL to redirect if the payment is successful
        public string? FailureUrl { get; set; }  // URL to redirect if the payment fails
    }
}