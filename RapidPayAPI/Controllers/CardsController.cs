using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RapidPayAPI.DTOs;
using RapidPayAPI.Models;
using System.Text.RegularExpressions;

namespace RapidPayAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly RapidPayDbContext _rapidPayDbContext;
        private readonly UFEService _feeService;
        public CardsController(RapidPayDbContext rapidPayDbContext, UFEService feeService)
        {
            this._rapidPayDbContext = rapidPayDbContext;
            this._feeService = feeService;
        }

        [HttpGet("{cardNumber}")]
        public async Task<IActionResult> GetCard(string cardNumber)
        {
            var card = await  this._rapidPayDbContext.Cards.FirstOrDefaultAsync(f=>f.CardNumber== cardNumber);
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCard([FromBody] Card card)
        {
            if (card == null)
                return BadRequest();

            if (card.CardNumber == null || card.CardNumber.Length != 15)
                return BadRequest();

            if (!Regex.IsMatch(card.CardNumber, @"^\d+${15}"))
                return BadRequest();

            using var transaction = this._rapidPayDbContext.Database.BeginTransaction();
            try
            {
                await this._rapidPayDbContext.Cards.AddAsync(card);
                await this._rapidPayDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPatch("{cardNumber}")]
        public async Task<IActionResult> PayCard(string cardNumber, [FromBody] PayDTO payment)
        {
            var fee = this._feeService.GetFee();
            using var transaction = this._rapidPayDbContext.Database.BeginTransaction();
            try
            {
                var card = await this._rapidPayDbContext.Cards.FirstOrDefaultAsync(f=>f.CardNumber== cardNumber);
                if (card == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound();
                }
                else
                {
                    card.Balance = card.Balance - (payment.Amount + fee);
                    await this._rapidPayDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok();

                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
