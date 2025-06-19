using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;

namespace PaymentAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpPost("ReportPayments")]
        public async Task<IActionResult> GetPayments([FromBody] ReportPaymentRequest request)
        {
            var result = await _reportService.GetPaymentReport(request);
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpPost("ReportCardbalances")]
        public async Task<IActionResult> GetCardBalances([FromBody] ReportCardBalanceRequest request)
        {
            var result = await _reportService.GetCardBalanceReport(request);
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}
