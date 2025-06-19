namespace PaymentAPI.Application.DTOs
{
    public class PaymentRequest
    {
        public string? CardNumber { get; set; }
        public decimal TransactionAmount { get; set; }
        public string? ReferenceId { get; set; }
    }
}
