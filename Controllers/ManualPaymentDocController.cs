using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SaleafApi.Models;
using SaleafApi.Models.DTO;
using SaleafApi.Repositories;
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
        public ManualPaymentDocController(IRepository<ManualPaymentDoc> repository,
        IS3Service S3Service, IConfiguration configuration, IDonation donation)
        {
            _S3Service = S3Service;
            _repository = repository;
            _awsRegion = configuration["AWS_REGION"]!;  
            _bucketName = configuration["AWS_BUCKET_NAME"]!;
            _donation = donation;
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
                return StatusCode(500, new { Message = "An error occurred while uploading the document to S3.", Details = s3Ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while processing your request.", Details = ex.Message });
            }
        }
    }
}