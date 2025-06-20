using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Application.Services;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;       
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;           
        }
        [Authorize]
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.ProcessPayment(request.CardNumber!, request.ReferenceId!, request.TransactionAmount);
            return Ok(new { result.ReferenceId, result.RefundCode, result.Message });
        }
    }
}
