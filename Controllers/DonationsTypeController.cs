using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers
{

    [ApiController]
    [Route("api/DonationType")]
    public class DonationsTypeController : ControllerBase
    {
        private readonly IRepository<DonationType> _donationType;
        public DonationsTypeController(IRepository<DonationType> donationType)
        {
            _donationType = donationType;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var donationsType = await _donationType.GetAllAsync();
            if (donationsType == null)
            {
                return NoContent();
            }
            return Ok(donationsType);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDonationType([FromBody] DonationTypeDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Fill in all the inputs.");
            }

            var donation = new DonationType
            {
                DonationsName = model.DonationsName,
                DonationsDescription = model.DonationsDescription,
                DonationAmount = model.DonationAmount
            };

            await _donationType.AddAsync(donation);
            return Ok($"Donation Type {model.DonationsName} has been added successfully.");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateDonationType(int id, [FromBody] DonationTypeDTO model)
        {
            var donation = await _donationType.GetByIdAsync(id);

            donation.DonationsName = model.DonationsName;
            donation.DonationsDescription = model.DonationsDescription;
            donation.DonationAmount = model.DonationAmount;
            donation.isEnable = model.isEnable;

            await _donationType.UpdateAsync(donation);
            return Ok($"Donation Type {model.DonationsName} has been updated successfully.");
        }
    }
}