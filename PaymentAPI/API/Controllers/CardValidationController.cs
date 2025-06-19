using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardValidationController : ControllerBase
    {
        private readonly ICardValidation _cardValidationService;
        public CardValidationController(ICardValidation cardValidationService)
        {
            _cardValidationService = cardValidationService;
           
        }

        [HttpPost("ValidateCard")]
        public async Task<IActionResult> ValidateCard([FromBody] CardValidationRequest request)
        {
            var isValidCard = await _cardValidationService.ValidateCard(request.CardNumber!, request.ExpiryMonth, request.ExpiryYear);
            return Ok(new { valid = isValidCard });
        }
    }

}
