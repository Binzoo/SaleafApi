using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeleafAPI.Models;

namespace SeleafAPI.Interfaces
{
    public interface IPayment
    {
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<string> InitiateCheckoutAsync(Donation donation, string cancelUrl, string successUrl, string failureUrl);
        Task<Donation> GetDonationByPaymentIdAsync(string paymentId);
        Task UpdateDonationStatusAsync(string paymentId, bool isPaid);
        Task<IEnumerable<DonationType>> GetDonationTypesAsync();
        Task<string> GetAppUserIdByPaymentId(string paymentId);

    }
}