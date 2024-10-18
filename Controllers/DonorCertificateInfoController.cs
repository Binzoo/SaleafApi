using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Models;
using SaleafApi.Models.DTO;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SaleafApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonorCertificateInfoController : ControllerBase
    {
        private readonly IRepository<DonorCertificateInfo> _repository;
        private readonly AppDbContext _donorCertificateInfo;
        public DonorCertificateInfoController(IRepository<DonorCertificateInfo> repository, AppDbContext donorCertificateInfo)
        {
            _repository = repository;
            _donorCertificateInfo = donorCertificateInfo;
        }

        [Authorize]
        [HttpPost("create-donor-certificate-info")]
        public async Task<IActionResult> CreateDonorCertificateInfo([FromBody] DonorCertificateInfoDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("userId")?.Value;
            var userDonorCertifiateInfo = _donorCertificateInfo.DonorCertificateInfos.FirstOrDefault(e => e.AppUserId == userId);

            if (userDonorCertifiateInfo != null)
            {
                return Conflict(new { message = "Resource already exists." });
            }

            DonorCertificateInfo donorCertificate = new DonorCertificateInfo
            {
                IdentityNoOrCompanyRegNo = model.IdentityNoOrCompanyRegNo,
                IncomeTaxNumber = model.IncomeTaxNumber,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                AppUserId = userId
            };

            await _repository.AddAsync(donorCertificate);
            return Ok("Donor Certificate Information Added Successfully.");
        }

        [Authorize]
        [HttpGet("donor-certificate-info-exist")]
        public async Task<IActionResult> DonorCertificateInfoExist()
        {
            var userId = User.FindFirst("userId")?.Value;
            var certificateInfo = await _donorCertificateInfo.DonorCertificateInfos.FirstOrDefaultAsync(e => e.AppUserId == userId);

            if (certificateInfo == null)
            {
                return Ok(false);
            }
            return Ok(true);
        }

        [Authorize]
        [HttpPut("update-donor-certificate-info")]
        public async Task<IActionResult> UpdateDonorCertificateInfo([FromBody] DonorCertificateInfoDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirst("userId")?.Value;
            var userDonorCertifiateInfo = _donorCertificateInfo.DonorCertificateInfos.FirstOrDefault(e => e.AppUserId == userId);
            if (userDonorCertifiateInfo == null)
                return BadRequest("User do not have donor certificate information.");

            userDonorCertifiateInfo.IdentityNoOrCompanyRegNo = model.IdentityNoOrCompanyRegNo;
            userDonorCertifiateInfo.IncomeTaxNumber = model.IncomeTaxNumber;
            userDonorCertifiateInfo.Address = model.Address;
            userDonorCertifiateInfo.PhoneNumber = model.PhoneNumber;

            await _repository.UpdateAsync(userDonorCertifiateInfo);
            return Ok("Donor Certificate Info Update Successfully.");
        }

    }
}