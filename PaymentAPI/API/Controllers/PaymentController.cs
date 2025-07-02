using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Application.Services;
using PaymentAPI.Domain.Events;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMessageBusPublisher _messageBusPublisher;

        public PaymentController(IPaymentService paymentService,IMessageBusPublisher messageBusPublisher)
        {
            _paymentService = paymentService;
            _messageBusPublisher = messageBusPublisher;
        }
        [Authorize]
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            // RabbitMq Implementation
            //var result = await _paymentService.ProcessPayment(request.CardNumber!, request.ReferenceId!, request.TransactionAmount);
            //if (result.Message=="Success")
            //{
            //    var evt = new PaymentSuccessEvent
            //    {
            //        CustomerEmail = "anuprasadkwt@gmail.com",
            //        CustomerName = "AnuPrasad",
            //        Amount = request.TransactionAmount
            //    };

            //    _messageBusPublisher.PublishPaymentSuccess(evt);

            //    return Ok(new { message = "Payment processed and event published." });
            //}
            //return Ok(new { result.ReferenceId, result.RefundCode, result.Message });
            var result = await _paymentService.ProcessPayment(request.CardNumber!, request.ReferenceId!, request.TransactionAmount);
            return Ok(new { result.ReferenceId, result.RefundCode, result.Message });
        }
    }
}
