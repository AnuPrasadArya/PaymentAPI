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
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;
        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }
        [Authorize]
        [HttpPost("ProcessRefund")]
        public async Task<IActionResult> ProcessRefund([FromBody] RefundRequest request)
        {
            var result = await _refundService.ProcessRefund(request.ReferenceId!, request.RefundCode!);
            return Ok(new { result.ReferenceId, result.RefundCode, result.Message });
        }
    }
}
