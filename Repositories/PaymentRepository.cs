using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Repositories
{
    public class PaymentRepository : IPayment
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;


        public PaymentRepository(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<string> InitiateCheckoutAsync(Donation donation, string cancelUrl, string successUrl, string failureUrl)
        {

            var builder = WebApplication.CreateBuilder();
            var yocoSecretKey = builder.Configuration["Yoco:SecretKey"];

            var checkoutData = new
            {
                amount = donation.Amount * 100,
                currency = donation.Currency,
                cancelUrl = cancelUrl,
                successUrl = successUrl,
                failureUrl = failureUrl,
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(checkoutData),
                Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {yocoSecretKey}");

            try
            {
                var requestUri = "https://payments.yoco.com/api/checkouts";
                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonDocument.Parse(responseContent);
                    var redirectUrl = jsonResponse.RootElement.GetProperty("redirectUrl").GetString();

                    var paymentId = jsonResponse.RootElement.GetProperty("id").GetString();
                    donation.PaymentId = paymentId;
                    await _context.SaveChangesAsync();

                    return redirectUrl!;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Yoco API Error: {errorContent}");
                    throw new Exception($"Failed to initiate checkout: {errorContent}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initiating checkout: {ex.Message}");
                throw;
            }
        }


        public async Task<string> GetAppUserIdByPaymentId(string paymentId)
        {
            var donation = await _context.Donations.FirstOrDefaultAsync(e => e.PaymentId == paymentId);

            if (donation == null)
            {
                return null!;
            }

            return donation.AppUserId!;
        }
    }

}