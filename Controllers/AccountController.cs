using Bank_API_EF.Dtos;
using BankApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace BankApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;


        public AccountsController(IAccountService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            try
            {
                var created = await _service.CreateAccountAsync(dto);
                return CreatedAtAction(
                    nameof(GetByAccountNumber),
                    new { accountNumber = created.AccountNumber },
                    new
                    {
                        message = "Account created successfully",
                        account = created
                    }
                );
            }

            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAccountsAsync();
            return Ok(list);
        }


        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(long accountNumber)
        {
            var acct = await _service.GetByAccountNumberAsync(accountNumber);
            if (acct == null) return NotFound();
            return Ok(acct);
        }


        [HttpGet("{accountNumber}/balance")]
        public async Task<IActionResult> GetBalance(long accountNumber)
        {
            var acct = await _service.GetByAccountNumberAsync(accountNumber);
            if (acct == null) return NotFound();
            return Ok(new
            {
                message = "Balance retrieved successfully",
                accountNumber = acct.AccountNumber,
                balance = acct.Balance
            });
        }



        [HttpPut("{accountNumber}")]
        public async Task<IActionResult> Update(long accountNumber, [FromBody] AccountUpdateDto dto)
        {
            var updated = await _service.UpdateAccountAsync(accountNumber, dto);
            if (updated == null) return NotFound();
            return Ok(new
            {
                success = true,
                message = "Account update successfully",
                updated
            });
        }


        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto dto)
        {
            try
            {
                var ok = await _service.DepositAsync(dto);
                if (!ok) return NotFound();
                return Ok(new
                {
                    success = true,
                    message = "Cash deposited successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawDto dto)
        {
            try
            {
                var ok = await _service.WithdrawAsync(dto);
                if (!ok) return BadRequest(new { error = "Invalid account, pin, or insufficient funds" });
                return Ok(new
                {
                    success = true,
                    message = "Cash withdrawn successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            try
            {
                var ok = await _service.TransferAsync(dto);
                if (!ok) return BadRequest(new { error = "Invalid data, pin, or insufficient funds" });
                return Ok(new
                {
                    success = true,
                    message = "Cash transfered successfully",
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update-pin")]
        public async Task<IActionResult> UpdatePin([FromBody] UpdatePinDto dto)
        {
            var success = await _service.UpdatePinAsync(dto);
            if (!success)
                return BadRequest(new { error = "Invalid account or old PIN" });

            return Ok(new
            {
                success = true,
                message = "PIN updated successfully"
            });
        }

        [HttpGet("{accountNumber}/transactions")]
        public async Task<IActionResult> GetTransactionHistory(long accountNumber)
        {
            var history = await _service.GetTransactionHistoryAsync(accountNumber);
            return Ok(new
            {
                success = true,
                message = "Account transaction recieved successfully",
                history
            });
        }


        [HttpPost("{accountNumber}/delete")]
        public async Task<IActionResult> Delete(long accountNumber, [FromBody] DeleteAccountDto dto)
        {
            var ok = await _service.DeleteAccountAsync(accountNumber, dto.PinHash);
            if (!ok) return BadRequest(new { error = "Invalid account or pin" });
            return Ok(new
            {
                success = true,
                message = "Account deleted successfully"
            });
        }
    }
}