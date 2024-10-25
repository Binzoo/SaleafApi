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
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] BankAccountInfoDTO model)
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BankAccountInfoDTO model)
        {
            var existingBankInfo = await _bankAccountInfo.GetByIdAsync(id);

            if (existingBankInfo == null)
            {
                return NotFound();
            }
            existingBankInfo.AccountNo = model.AccountNo;
            existingBankInfo.BranchCode = model.BranchCode;
            existingBankInfo.Branch = model.Branch;
            
            await _bankAccountInfo.UpdateAsync(existingBankInfo);
            return Ok(new
            {
                message = "BankInfo updated successfully."
            });
        }
    }
}