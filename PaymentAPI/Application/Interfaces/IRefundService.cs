namespace PaymentAPI.Application.Interfaces
{
    public interface IRefundService
    {
        Task<(string ReferenceId, string RefundCode, string Message)> ProcessRefund(string TransactionId, string RefundCode);

    }
}
