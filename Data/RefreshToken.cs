using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeleafAPI.Data;

namespace SaleafApi.Data
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public bool IsRevoked { get; set; }
    }
}