using Microsoft.AspNetCore.Identity;
using SeleafAPI.Models;

namespace SeleafAPI.Data
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public bool isStudent { get; set; }
        public bool isVerified { get; set; }

        public ICollection<Donation>? Donations { get; set; }
    }
}

