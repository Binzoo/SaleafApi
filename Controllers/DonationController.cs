using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SaleafApi.Interfaces;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationController : ControllerBase
    {
        private readonly IPayment _payment;
        private readonly IDonation _donation;
        public DonationController(IPayment payment, IDonation donation)
        {
            _payment = payment;
            _donation = donation;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonation([FromBody] DonationDTO request)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in claims.");
            }
            var donation = new Donation
            {
                Amount = request.Amount,
                Currency = request.Currency,
                AppUserId = userId,
                DonationTypeId = request.DonationTypeId,
                IsPaid = false,
                isAnonymous = request.isAnonymous
            };

            try
            {
                var redirectUrl = await _payment.InitiateCheckoutAsync(await _donation.CreateDonationAsync
                (donation), request.CancelUrl!, request.SuccessUrl!, request.FailureUrl!);

                return Ok(new { redirectUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing the donation: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetDonations()
        {
            var donations = await _donation.GetDonations();

            var model = donations.Select(d => new
            {
                UserName = d.AppUser?.UserName ?? "Unknown",
                FirstName = d.AppUser?.FirstName ?? "Unknown",
                LastName = d.AppUser?.LastName ?? "Unknown",
                DonationTypeName = d.DonationType?.DonationsName ?? "No Type",
                d.PaymentId,
                d.Amount,
                d.Currency,
                d.CreatedAt,
                d.IsPaid,
                d.isAnonymous
            }).ToList();

            return Ok(model);
        }

    }
}