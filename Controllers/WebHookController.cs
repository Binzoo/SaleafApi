using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using SaleafApi.Interfaces;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

using QRCoder;
using SeleafAPI.Models.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;



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
        private readonly IS3Service _S3Service;
        private readonly string _awsRegion;
        private readonly string _bucketName;

        public WebHookController(IDonation donationRepository, IEmailSender emailService, IUserRepository user, IPayment paymentRepository, IPdf pdf, AppDbContext donorCertificateInfo,
            IEventRegistration eventRegistration,  IS3Service S3Service, IConfiguration configuration)
        {
            _donationRepository = donationRepository;
            _emailService = emailService;
            _user = user;
            _paymentRepository = paymentRepository;
            _pdf = pdf;
            _donationRepository = donationRepository;
            _donorCertificateInfo = donorCertificateInfo;
            _eventRegistration = eventRegistration;
            _S3Service = S3Service;
             _awsRegion = configuration["AWS_REGION"];  
            _bucketName = configuration["AWS_BUCKET_NAME"]; 
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
                var checkoutId = webhookEvent.Payload!.Metadata!.CheckoutId;

                var eventreg = await _eventRegistration.GetEventRegistrationsByPaymentIdAsync(checkoutId);

                if (eventreg != null)
                {
                    await _eventRegistration.UpdateEventRegistrationsStatusAsync(checkoutId!, true);
                    var userIdEvent = await _paymentRepository.GetAppUserIdByPaymentIdEvent(checkoutId!);
                    var paidEventUser = await _user.FindByIdAsync(userIdEvent);
                    var eventUser = await _eventRegistration.GetEventRegistrationsByPaymentIdAsync(checkoutId!);
                   
                    if (paidEventUser == null)
                    {
                        return BadRequest("No user found.");
                    }
                    
                    // Generate QR code bytes
                    byte[] qrCodeBytes;
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(eventUser), QRCodeGenerator.ECCLevel.Q);
                        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                        qrCodeBytes = qrCode.GetGraphic(20);
                    }
                    
                    var registrationInfo = new EventRegistrationDTO
                    {
                        FristName = paidEventUser.FirstName,
                        LastName = paidEventUser.LastName,
                        PhoneNumber = paidEventUser.PhoneNumber,
                        NumberOfParticipant = eventUser.NumberOfParticipant
                    };
                    
                    
                    var pdfStream2 = _pdf.GenerateEventPdfWithQrCode(registrationInfo, qrCodeBytes);
                        
                    await _emailService.SendEmailAsyncWithAttachment(
                        paidEventUser.Email,
                        "Event Registration Details",
                        SuccessEventEmail(),
                        pdfStream2, "Event Registration Details.pdf"
                    );
                     
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
                await _emailService.SendEmailAsyncWithAttachment(paidUser.Email!, "SALEAF", body, pdfStream, "Section 18A.pdf");
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
    string body = $@"
    <html>
    <head>
        <style>
            .email-content {{
                font-family: Arial, sans-serif;
                color: #333;
                max-width: 600px;
                margin: auto;
                border: 1px solid #ddd;
                border-radius: 8px;
                overflow: hidden;
            }}
            .email-header {{
                background-color: #4CAF50;
                color: white;
                padding: 20px;
                text-align: center;
            }}
            .email-body {{
                padding: 20px;
                text-align: left;
            }}
            .email-footer {{
                background-color: #f8f8f8;
                padding: 10px;
                text-align: center;
                font-size: 12px;
                color: #888;
                border-top: 1px solid #ddd;
            }}
            .button {{
                display: inline-block;
                background-color: #28a745;
                color: white;
                padding: 10px 20px;
                text-decoration: none;
                font-size: 14px;
                border-radius: 5px;
                text-align: center;
            }}
            a.button:hover {{
                background-color: #218838;
            }}
        </style>
    </head>
    <body>
        <div class='email-content'>
            <div class='email-header'>
                <h1>Event Registration Successful!</h1>
            </div>
            <div class='email-body'>
                <p>Dear Valued Participant,</p>
                <p>Thank you for registering for our event! We are thrilled to have you join us and look forward to an amazing experience together.</p>
                <p>Your QR code for the event has been attached to this email as a PDF. Please download and keep it safe for event check-in.</p>
                <p>If you have any questions or need further assistance, please <a href='#'>contact us</a>.</p>
                <p>Best regards,<br/>The SALEAF Team</p>
                <div style='text-align: center; margin-top: 20px;'>
                    <a href='#' class='button'>View Event Details</a>
                </div>
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