using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeleafAPI.Models;

namespace SeleafAPI.Interfaces
{
    public interface IPayment
    {
        Task<string> InitiateCheckoutAsync(Donation donation, string cancelUrl, string successUrl, string failureUrl);
        Task<string> GetAppUserIdByPaymentId(string paymentId);
    }
}