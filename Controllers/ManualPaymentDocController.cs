using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Interfaces;
using SaleafApi.Models;
using SaleafApi.Models.DTO;
using SeleafAPI.Controllers;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SaleafApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManualPaymentDocController : ControllerBase
    {
        private readonly IRepository<ManualPaymentDoc> _repository;
        private readonly IS3Service _S3Service;
        private readonly string _awsRegion;
        private readonly string _bucketName;
        private readonly IDonation _donation;
        private readonly IEmailSender _emailService;
        private readonly AppDbContext _context;
        private readonly IPdf _pdf;

        public ManualPaymentDocController(IRepository<ManualPaymentDoc> repository,
            IS3Service S3Service, IConfiguration configuration, IDonation donation, IEmailSender emailService,
            AppDbContext context, IPdf pdf)
        {
            _S3Service = S3Service;
            _repository = repository;
            _awsRegion = configuration["AWS_REGION"]!;
            _bucketName = configuration["AWS_BUCKET_NAME"]!;
            _donation = donation;
            _emailService = emailService;
            _context = context;
            _pdf = pdf;
        }

        [HttpPost("upload-manual-payment")]
        public async Task<IActionResult> Post([FromForm] ManualPaymentDocDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid request. Please check the data provided." });
            }

            if (model.DocFile == null || model.DocFile.Length == 0)
            {
                return BadRequest(new { Message = "File not provided. Please upload a valid document." });
            }

            var donation = await _donation.GetDonationById(model.ReferenceNumber);
            if (donation == null)
            {
                return BadRequest(new { Message = "No donation found for the provided reference number." });
            }

            var fileName = $"proofOfPayments/{Guid.NewGuid()}-{model.DocFile.FileName}";

            try
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    await model.DocFile.CopyToAsync(newMemoryStream);
                    await _S3Service.UploadFileAsync(newMemoryStream, fileName);
                }

                var s3Url = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";

                var manualPayment = new ManualPaymentDoc
                {
                    ReferenceNumber = model.ReferenceNumber,
                    DocUrl = s3Url,
                    Amount = model.Amount,
                    Checked = false
                };

                await _repository.AddAsync(manualPayment);

                return Ok(new { Message = "Document uploaded successfully.", DocUrl = s3Url });
            }
            catch (AmazonS3Exception s3Ex)
            {
                return StatusCode(500,
                    new { message = "An error occurred while uploading the document to S3.", Details = s3Ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "An unexpected error occurred while processing your request.", Details = ex.Message
                    });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-manual-payment-by-reference-number/{referenceNumber}")]
        public async Task<IActionResult> GetByReferenceNumber(int referenceNumber)
        {
            try
            {
                var getByReference = await _context.ManualPaymentDocs.Where(e => e.ReferenceNumber == referenceNumber)
                    .FirstOrDefaultAsync();

                if (getByReference == null)
                {
                    return NotFound(new { Message = "Reference not found." });
                }

                return Ok(getByReference);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "An unexpected error occurred while processing your request.", Details = ex.Message
                    }); 
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getAll = await _repository.GetAllAsync();   
            return Ok(getAll);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("accept-reject-manual-payment")]
        public async Task<IActionResult> RejectManualPayment([FromQuery] int referenceNo, string reason, int statusCode)
        {
            try
            {
                var manualpayment = await _context.ManualPaymentDocs.Where(d => d.ReferenceNumber == referenceNo)
                    .FirstOrDefaultAsync();
                if (manualpayment == null)
                {
                    return NotFound(
                        new { message = $"No manual payment found for the reference number {referenceNo}." });
                }

                if (manualpayment.Checked)
                {
                    return BadRequest(new { message = "You have already accepted this manual payment." });
                }
                var donationWithUser = await _context.Donations
                    .Include(d => d.AppUser)
                    .FirstOrDefaultAsync(d => d.Id == referenceNo);

                var userId = donationWithUser.AppUser.Id;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                
                if (statusCode == 1)
                {
                    var certificate = await _context.DonorCertificateInfos.FirstOrDefaultAsync(e => e.AppUserId == userId);

                    manualpayment.Checked = true;
                    await _repository.UpdateAsync(manualpayment);
                    
                    if (certificate == null)
                    {
                        return NotFound(
                            new
                            {
                                message = $"No certificate found for the user {user.FirstName} {user.LastName}.",
                            });
                    }

                    AllDonorCertificateInfo allDonorCertificate = new AllDonorCertificateInfo
                    {
                        RefNo = referenceNo,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IdentityNoOrCompanyRegNo = certificate!.IdentityNoOrCompanyRegNo,
                        IncomeTaxNumber = certificate.IncomeTaxNumber,
                        Address = certificate.Address,
                        PhoneNumber = certificate.PhoneNumber,
                        Amount = donationWithUser.Amount,
                        Email = user.Email,
                        DateofReceiptofDonation = donationWithUser.CreatedAt
                    };
                    var pdfStream = _pdf.GetPdf(allDonorCertificate);
                    await _emailService.SendEmailAsyncWithAttachment(user.Email!, "Manual Payment accepted.", Body(),
                        pdfStream);
                    return Ok(new { message = "Manual payment accepted." });
                    
                }
                else
                {
                    await _emailService.SendEmailAsync(user.Email!, "Manual Payment rejected.", RejectionBody(reason));
                    return Ok(new { message = "Manual payment rejected." });    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new
                    {
                        message = "An unexpected error occurred while processing your request.", Details = ex.Message
                    });
            }
        }
    private string Body()
        {
            string body = @"<html>
                                    <head>
                                        <style>
                                            .email-content {
                                                font-family: Arial, sans-serif;
                                                color: #333;
                                            }
                                            .email-header {
                                                background-color: #f0f8ff;
                                                padding: 20px;
                                                text-align: center;
                                                border-bottom: 1px solid #ddd;
                                            }
                                            .email-body {
                                                padding: 20px;
                                            }
                                            .email-footer {
                                                background-color: #f0f8ff;
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
                                                <h1>Payment Accepted</h1>
                                            </div>
                                            <div class='email-body'>
                                                <p>Dear valued supporter,</p>
                                                <p>We are pleased to inform you that your manual payment has been successfully accepted.</p>
                                                <p>Thank you for your continued support and generosity. Your contribution is greatly appreciated and makes a significant difference.</p>
                                                <p>Feel free to reach out if you have any questions or need assistance.</p>
                                                <p>Best regards,<br/>The SALEAF Team</p>
                                                <a href='#' class='button'>View Your Account</a>
                                            </div>
                                            <div class='email-footer'>
                                                <p>&copy; 2024 SALEAF. All rights reserved.</p>
                                            </div>
                                        </div>
                                    </body>
                                </html>";

            return body;

        }
        
        private string RejectionBody(string rejectionReason)
{
    string body = @"<html>
                                    <head>
                                        <style>
                                            .email-content {
                                                font-family: Arial, sans-serif;
                                                color: #333;
                                            }
                                            .email-header {
                                                background-color: #f8d7da;
                                                padding: 20px;
                                                text-align: center;
                                                border-bottom: 1px solid #ddd;
                                                color: #721c24;
                                            }
                                            .email-body {
                                                padding: 20px;
                                            }
                                            .email-footer {
                                                background-color: #f8d7da;
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
                                                <h1>Payment Rejected</h1>
                                            </div>
                                            <div class='email-body'>
                                                <p>Dear valued supporter,</p>
                                                <p>We regret to inform you that your manual payment has been <strong>rejected</strong> due to the following reason:</p>
                                                <p><strong>"+ rejectionReason+ @"</strong></p>
                                                <p>If you believe this is an error or if you have any questions, please do not hesitate to contact us for assistance.</p>
                                                <p>We value your support and look forward to resolving this matter promptly.</p>
                                                <p>Best regards,<br/>The SALEAF Team</p>
                                                <a href='#' class='button'>Contact Us</a>
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