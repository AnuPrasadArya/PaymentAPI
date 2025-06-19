namespace PaymentAPI.Application.DTOs
{
    public class ReportPaymentRequest
    {
        public string? CardNumber { get; set; }
        public string? ReferenceId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
