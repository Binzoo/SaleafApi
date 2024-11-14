using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Interfaces;
using SaleafApi.Models;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Controllers
{
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IDonation _donationRepository;
        private readonly IEventRegistration _eventRegistration;
        private readonly IEmailSender _emailService;
        private readonly IUserRepository _user;
        private readonly IPayment _paymentRepository;
        private readonly IPdf _pdf;
        private readonly AppDbContext _donorCertificateInfo;

        public WebHookController(IDonation donationRepository, IEmailSender emailService, IUserRepository user, IPayment paymentRepository, IPdf pdf, AppDbContext donorCertificateInfo,
            IEventRegistration eventRegistration)
        {
            _donationRepository = donationRepository;
            _emailService = emailService;
            _user = user;
            _paymentRepository = paymentRepository;
            _pdf = pdf;
            _donationRepository = donationRepository;
            _donorCertificateInfo = donorCertificateInfo;
            _eventRegistration = eventRegistration;
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

                var eventreg = await _eventRegistration.GetEventRegistrationsByPaymentIdAsync(checkoutId);

                if (eventreg != null)
                {
                    await _eventRegistration.UpdateEventRegistrationsStatusAsync(checkoutId!, true);
                    var userIdEvent = await _paymentRepository.GetAppUserIdByPaymentIdEvent(checkoutId!);
                    var paidEventUser = await _user.FindByIdAsync(userIdEvent);

                    if (paidEventUser == null)
                    {
                        return BadRequest("No user found.");
                    }

                    string eventBody = SuccessEventEmail();

                    await _emailService.SendEmailAsync(paidEventUser.Email, "Thank you for registering for event.",eventBody);
                    return Ok();
                }
                
                // Update the donation status to "Paid"
                await _donationRepository.UpdateDonationStatusAsync(checkoutId!, true);
                var userId = await _paymentRepository.GetAppUserIdByPaymentId(checkoutId!);
                var paidUser = await _user.FindByIdAsync(userId);
                if (paidUser == null)
                {
                    return BadRequest("No user found.");
                }

                string body = SuccessEmail();
                
                var certificateInfo = await _donorCertificateInfo.DonorCertificateInfos.FirstOrDefaultAsync(e => e.AppUserId == userId);
                var donationInfo = await _donationRepository.GetDonationByPaymentIdAsync(checkoutId!);

                AllDonorCertificateInfo allDonorCertificate = new AllDonorCertificateInfo
                {
                    RefNo = donationInfo.Id,
                    FirstName = paidUser.FirstName,
                    LastName = paidUser.LastName,
                    IdentityNoOrCompanyRegNo = certificateInfo!.IdentityNoOrCompanyRegNo,
                    IncomeTaxNumber = certificateInfo.IncomeTaxNumber,
                    Address = certificateInfo.Address,
                    PhoneNumber = certificateInfo.PhoneNumber,
                    Amount = donationInfo.Amount,
                    Email = paidUser.Email!,
                    DateofReceiptofDonation = donationInfo.CreatedAt
                };

                var pdfStream = _pdf.GetPdf(allDonorCertificate);
                await _emailService.SendEmailAsyncWithAttachment(paidUser.Email!, "SALEAF", body, pdfStream);
            }
            else if (webhookEvent.Type == "payment.failed")
            {
                var checkoutId = webhookEvent.Payload!.Metadata!.CheckoutId;
                
                var eventreg = await _eventRegistration.GetEventRegistrationsByPaymentIdAsync(checkoutId);
                if (eventreg != null)
                {
                    await _eventRegistration.UpdateEventRegistrationsStatusAsync(checkoutId!, true);
                    var userIdEvent = await _paymentRepository.GetAppUserIdByPaymentIdEvent(checkoutId!);
                    var paidEventUser = await _user.FindByIdAsync(userIdEvent);

                    if (paidEventUser == null)
                    {
                        return BadRequest("No user found.");
                    }

                    string eventBody = FailedEventEmail();

                    await _emailService.SendEmailAsync(paidEventUser.Email, "Thank you for registering for event.",eventBody);
                    return Ok();
                }
                var userId = await _paymentRepository.GetAppUserIdByPaymentId(checkoutId!);
                var paidUser = await _user.FindByIdAsync(userId);
                if (paidUser == null)
                {
                    return BadRequest("No user found.");
                }
                
                string body = BadEmail();
                

                await _emailService.SendEmailAsync(paidUser.Email!, "SALEAF", body);
            }
            else
            {
                return BadRequest($"Unhandled event type: {webhookEvent.Type}");
            }
            return Ok();
        }
        
        private string FailedEventEmail()
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
                                        background-color: #dc3545;
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
                                        <h1>Event Registration Failed</h1>
                                    </div>
                                    <div class='email-body'>
                                        <p>Dear valued participant,</p>
                                        <p>Unfortunately, your event registration was not successful.</p>
                                        <p>We apologize for any inconvenience caused and invite you to try registering again.</p>
                                        <p>If you continue to experience issues or have any questions, please <a href='#'>contact us</a>.</p>
                                        <p>Best regards,<br/>The SALEAF Team</p>
                                        <a href='#' class='button'>Try Again</a>
                                    </div>
                                    <div class='email-footer'>
                                        <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";
    return body;
}

        
        private  string SuccessEmail()
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
        return body;
    }
        
        private string SuccessEventEmail()
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
                                        <h1>Event Registration Successful!</h1>
                                    </div>
                                    <div class='email-body'>
                                        <p>Dear valued participant,</p>
                                        <p>Thank you for registering for our event.</p>
                                        <p>We are excited to have you join us and look forward to an amazing experience together.</p>
                                        <p>For more details or if you have any questions, please <a href='#'>contact us</a>.</p>
                                        <p>Best regards,<br/>The SALEAF Team</p>
                                        <a href='#' class='button'>View Event Details</a>
                                    </div>
                                    <div class='email-footer'>
                                        <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";
    return body;
}


        private string BadEmail()
        {
            string body = @"<html>
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
                                                background-color: #dc3545;
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
                                                <h1>Payment Failed</h1>
                                            </div>
                                            <div class='email-body'>
                                                <p>Dear valued supporter,</p>
                                                <p>We regret to inform you that your payment has not been successful.</p>
                                                <p>Please try again or contact us if you have any questions or concerns.</p>
                                                <p>Your support means a lot to us, and we hope to resolve this issue soon.</p>
                                                <p>Best regards,<br/>The SALEAF Team</p>
                                                <a href='#' class='button'>Retry Payment</a>
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
    
    
    public class AllDonorCertificateInfo
    {
        public int RefNo { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdentityNoOrCompanyRegNo { get; set; }
        public string? IncomeTaxNumber { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public double Amount { get; set; }
        public string? Email { get; set; }
        public DateTime DateofReceiptofDonation { get; set; }
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