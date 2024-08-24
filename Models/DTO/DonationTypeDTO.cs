using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeleafAPI.Models.DTO
{
    public class DonationTypeDTO
    {
        [Required]
        public string? DonationsName { get; set; }
        public string? DonationsDescription { get; set; }
        [Required]
        public int DonationAmount { get; set; }
        public bool isEnable { get; set; } = true;
    }
}