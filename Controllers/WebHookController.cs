using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Controllers
{
    [ApiController]
    public class WebHookController : ControllerBase
    {

        private readonly IDonation _donationRepository;
        private readonly IEmailSender _emailService;
        private readonly IUserRepository _user;
        private readonly IPayment _paymentRepository;

        private readonly IPdf _pdf;


        public WebHookController(IDonation donationRepository, IEmailSender emailService, IUserRepository user, IPayment paymentRepository, IPdf pdf)
        {
            _donationRepository = donationRepository;
            _emailService = emailService;
            _user = user;
            _paymentRepository = paymentRepository;
            _pdf = pdf;
        }

        // POST: api/webhook/yoco
        [HttpPost("api/webhook/yoco")]
        public async Task<IActionResult> YocoWebhook([FromBody] YocoWebhookEvent webhookEvent)
        {
            System.Console.WriteLine(webhookEvent);
            if (webhookEvent == null || string.IsNullOrEmpty(webhookEvent.Type))
            {
                return BadRequest("Invalid webhook event data.");
            }

            if (webhookEvent.Type == "payment.succeeded")
            {
                var checkoutId = webhookEvent.Payload!.Metadata!.CheckoutId;
                // Update the donation status to "Paid"
                await _donationRepository.UpdateDonationStatusAsync(checkoutId!, true);
                var userId = await _paymentRepository.GetAppUserIdByPaymentId(checkoutId!);
                var paidUser = await _user.FindByIdAsync(userId);
                if (paidUser == null)
                {
                    return BadRequest("No user found.");
                }
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
                                        <h1>Thank You for Your Donation!</h1>
                                    </div>
                                    <div class='email-body'>
                                        <p>Dear valued supporter,</p>
                                        <p>We sincerely appreciate your generous donation.</p>
                                        <p>Your support helps us continue our work and make a positive impact.</p>
                                        <p>If you have any questions, feel free to <a href='#'>contact us</a>.</p>
                                        <p>Best regards,<br/>The SALEAF Team</p>
                                        <a href='#' class='button'>View Your Donation</a>
                                    </div>
                                    <div class='email-footer'>
                                        <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

                var pdfStream = _pdf.GetPdf();
                await _emailService.SendEmailAsyncWithAttachment(paidUser.Email!, "SALEAF", body, pdfStream);
            }
            else
            {
                return BadRequest($"Unhandled event type: {webhookEvent.Type}");
            }
            return Ok();
        }
    }

    public class YocoWebhookEvent
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public YocoPayload? Payload { get; set; }
    }

    public class YocoPayload
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public YocoPaymentMethodDetails? PaymentMethodDetails { get; set; }
        public string? Status { get; set; }
        public string? Mode { get; set; }
        public YocoMetadata? Metadata { get; set; }
    }

    public class YocoPaymentMethodDetails
    {
        public string? Type { get; set; }
        public YocoCardDetails? Card { get; set; }
    }

    public class YocoCardDetails
    {
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? MaskedCard { get; set; }
        public string? Scheme { get; set; }
    }

    public class YocoMetadata
    {
        public string? CheckoutId { get; set; }
    }


}