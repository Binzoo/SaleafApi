using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Controllers
{
    [ApiController]
    public class WebHookController : ControllerBase
    {

        private readonly IPayment _paymentRepository;
        private readonly IEmailSender _emailService;

        private readonly IUserRepository _user;


        public WebHookController(IPayment paymentRepository, IEmailSender emailService, IUserRepository user)
        {
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _user = user;
        }

        // POST: api/webhook/yoco
        [HttpPost("api/webhook/yoco")]
        public async Task<IActionResult> YocoWebhook([FromBody] YocoWebhookEvent webhookEvent)
        {

            if (webhookEvent == null || string.IsNullOrEmpty(webhookEvent.Type))
            {
                return BadRequest("Invalid webhook event data.");
            }

            if (webhookEvent.Type == "payment.succeeded")
            {
                var checkoutId = webhookEvent.Payload.Metadata.CheckoutId;
                // Update the donation status to "Paid"
                await _paymentRepository.UpdateDonationStatusAsync(checkoutId, true);
                var userId = await _paymentRepository.GetAppUserIdByPaymentId(checkoutId);
                var paidUser = await _user.FindByIdAsync(userId);
                if (paidUser == null)
                {
                    return BadRequest("No user found.");
                }

                await _emailService.SendEmailAsync(paidUser.Email!, "SALEAF", $"Thank you for Donation.");
            }
            else
            {
                // Handle other event types if necessary
                return BadRequest($"Unhandled event type: {webhookEvent.Type}");
            }
            return Ok();
        }


    }

    public class YocoWebhookEvent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public YocoPayload Payload { get; set; }
    }

    public class YocoPayload
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public YocoPaymentMethodDetails PaymentMethodDetails { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public YocoMetadata Metadata { get; set; }
    }

    public class YocoPaymentMethodDetails
    {
        public string Type { get; set; }
        public YocoCardDetails Card { get; set; }
    }

    public class YocoCardDetails
    {
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string MaskedCard { get; set; }
        public string Scheme { get; set; }
    }

    public class YocoMetadata
    {
        public string CheckoutId { get; set; }
    }


}