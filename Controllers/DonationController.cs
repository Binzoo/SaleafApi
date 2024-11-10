using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IEmailSender _emailService;
        private readonly IUserRepository _user;

        public DonationController(IPayment payment, IDonation donation, IUserRepository user, IEmailSender emailService)
        {
            _payment = payment;
            _donation = donation;
            _user = user;
            _emailService = emailService;
        }

        [Authorize]
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
        
        [Authorize]
        [HttpPost("manual-payment-donation")]
        public async Task<IActionResult> ManualPaymentDonation([FromBody] ManualPaymentDonationDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid donation request.");
            }

            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims.");
            }

            var donation = new Donation
            {
                Amount = request.Amount,
                Currency = request.Currency,
                AppUserId = userId,
                IsPaid = false,
                isAnonymous = request.isAnonymous,
                PaymentId = "Manual Payment"
            };

            try
            {
                await _donation.CreateDonationAsync(donation);
                var user = await _user.FindByIdAsync(userId);
                string referenceNo = donation.Id.ToString();

                await _emailService.SendEmailAsync(user.Email!, "Upload proof of payment of your manual payment.",
                returnManualPaymentHTMLFile(referenceNo));
                return Ok(new { Message = "Manual donation added successfully.", DonationId = donation.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the donation.", Details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetDonations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            
            var donations = await _donation.GetDonations();
            var totalItems = donations.Count(); 
            
            var paginatedDonations = donations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = paginatedDonations.Select(d => new
            {
                UserName = d.AppUser?.UserName ?? "Unknown",
                FirstName = d.AppUser?.FirstName ?? "Unknown",
                LastName = d.AppUser?.LastName ?? "Unknown",
                d.PaymentId,
                d.Amount,
                d.Currency,
                d.CreatedAt,
                d.IsPaid,
                d.isAnonymous
            }).ToList();

            var response = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                Data = model
            };

            return Ok(response);
        }


        private string returnManualPaymentHTMLFile(string referenceNo)
        {
            string body = @"
                            <html>
                            <head>
                                <style>
                                    .email-content {
                                        font-family: Arial, sans-serif;
                                        color: #333;
                                    }
                                    .email-header {
                                        background-color: #f8f8f8;
                                        padding: 20px;
                                        text-align: center;
                                        border-bottom: 1px solid #ddd;
                                    }
                                    .email-body {
                                        padding: 20px;
                                    }
                                    .email-footer {
                                        background-color: #f8f8f8;
                                        padding: 10px;
                                        text-align: center;
                                        font-size: 12px;
                                        color: #888;
                                        border-top: 1px solid #ddd;
                                    }
                                    .button {
                                        background-color: #28a745;
                                        color: white;
                                        padding: 10px 20px;
                                        text-align: center;
                                        text-decoration: none;
                                        display: inline-block;
                                        border-radius: 5px;
                                    }
                                </style>
                            </head>
                            <body>
                                <div class='email-content'>
                                    <div class='email-header'>
                                        <h1>Upload Proof of Payment for Your Manual Donation</h1>
                                    </div>
                                    <div class='email-body'>
                                        <p>Dear valued supporter,</p>
                                        <p>We sincerely appreciate your generous donation.</p>
                                        <p>To complete your manual payment, please upload the proof of payment using the link below.</p>
                                        <p>Your reference number is: " + referenceNo + @" </p>
                                        <p>If you have any questions, feel free to <a href='#'>contact us</a>.</p>
                                        <p>Best regards,<br/>The SALEAF Team</p>
                                        <a href='#' class='button'>Upload Proof of Payment</a>
                                    </div>
                                    <div class='email-footer'>
                                        <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            return body;
        }

    }
}