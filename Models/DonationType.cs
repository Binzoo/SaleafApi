using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Identity.Client;

namespace SeleafAPI.Models
{
    public class DonationType
    {
        [Key]
        public int DonationId { get; set; }
        public string? DonationsName { get; set; }
        public string? DonationsDescription { get; set; }
        public int DonationAmount { get; set; }
        public bool isEnable { get; set; } = true;
        public ICollection<Donation> Donations { get; set; }
    }
}