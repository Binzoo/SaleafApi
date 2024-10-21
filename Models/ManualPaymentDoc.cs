using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Models
{
    public class ManualPaymentDoc
    {
        public int Id { get; set; }
        public int ReferenceNumber { get; set; }
        public string DocUrl { get; set; }
        public double Amount { get; set; }
        public bool Checked { get; set; } = false;
    }
}