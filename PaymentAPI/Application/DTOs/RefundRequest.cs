namespace PaymentAPI.Application.DTOs
{
    public class RefundRequest
    {
        public string? ReferenceId { get; set; }
        public string? RefundCode { get; set; }
    }
}
