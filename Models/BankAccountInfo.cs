using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models
{
    public class BankAccountInfo
    {
        public int Id { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string AccountNo { get; set; }
    }
}