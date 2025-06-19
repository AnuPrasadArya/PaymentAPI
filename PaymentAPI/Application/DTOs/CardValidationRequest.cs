namespace PaymentAPI.Application.DTOs
{
    public class CardValidationRequest
    {
        public string? CardNumber { get; set; }
        public int CVV { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
    }
}
