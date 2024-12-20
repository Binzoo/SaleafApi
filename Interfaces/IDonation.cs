using SeleafAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleafApi.Interfaces
{
    public interface IDonation
    {
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<List<Donation>> GetDonations();
        Task<Donation> GetDonationByPaymentIdAsync(string paymentId);
        Task UpdateDonationStatusAsync(string paymentId, bool isPaid);
        Task<Donation> GetDonationById(int id);
        Task<int> GetTotalDonationsCountAsync();
        Task<List<Donation>> GetPaginatedDonations(int pageNumber, int pageSize);
        Task<List<Donation>> GetAllLoggedInUserDonations(string userId);
        Task UpdateDonationStatusPOPAsync(int referenceNo, bool isPaid);
    }
}