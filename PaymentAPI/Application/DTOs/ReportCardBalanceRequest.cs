namespace PaymentAPI.Application.DTOs
{
    public class ReportCardBalanceRequest
    {
        public string? CardHolder { get; set; }
        public string? CardNumber { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
