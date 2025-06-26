namespace PaymentAPI.Domain.Events
{
    public class PaymentSuccessEvent
    {
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public decimal Amount { get; set; }
    }
}
