using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models.DTO
{
    public class BankAccountInfoDTO
    {
        [Required]
        public string Branch { get; set; }
        [Required]
        public string BranchCode { get; set; }
        [Required]
        public string AccountNo { get; set; }
    }
}