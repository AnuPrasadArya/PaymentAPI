namespace PaymentAPI.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<(string ReferenceId, string RefundCode, string Message)> ProcessPayment(string CardNumber, string ReferenceId, decimal Amount);
    }
}
