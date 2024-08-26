using Microsoft.AspNetCore.Mvc;
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
        public DonationController(IPayment payment)
        {
            _payment = payment;
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
                isAnonymous = true
            };
            try
            {
                var redirectUrl = await _payment.InitiateCheckoutAsync(donation, request.CancelUrl, request.SuccessUrl, request.FailureUrl);
                return Ok(new { redirectUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing the donation: {ex.Message}");
            }
        }
    }
}