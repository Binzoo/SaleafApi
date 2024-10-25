using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Models;
using SaleafApi.Models.DTO;
using SeleafAPI.Interfaces;

namespace SaleafApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountInfoController : ControllerBase
    {
        private readonly IRepository<BankAccountInfo> _bankAccountInfo;

        public BankAccountInfoController(IRepository<BankAccountInfo> bankAccountInfo)
        {
            _bankAccountInfo = bankAccountInfo;
        }

        [Authorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BankAccountInfoDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Input all field.");
            }

            var bankaccountinfo = new BankAccountInfo
            {
                AccountNo = model.AccountNo,
                Branch = model.Branch,
                BranchCode = model.BranchCode
            };

            await _bankAccountInfo.AddAsync(bankaccountinfo);
            return Ok(new
            {
                message = "Bank added successfully."
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bankaccountinfos = await _bankAccountInfo.GetAllAsync();
            if (bankaccountinfos == null)
            {
                return NotFound();
            }
            return Ok(bankaccountinfos);
        }

        [Authorize("admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BankAccountInfoDTO model)
        {
            var bankinfo = new BankAccountInfo()
            {
                Branch = model.Branch,
                AccountNo = model.AccountNo,
                BranchCode = model.BranchCode
            };
            await _bankAccountInfo.UpdateAsync(bankinfo);
            return Ok(new
            {
                message = "BankInfo updated successfully."
            });
        }
        
        
    }
}