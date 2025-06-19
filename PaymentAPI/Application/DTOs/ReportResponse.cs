namespace PaymentAPI.Application.DTOs
{
    public class ReportResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        //public string TransactionId { get; set; } = default!;
        //public decimal Amount { get; set; }
        //public string TransactionStatus { get; set; } = default!;
        //public DateTime CreatedOn { get; set; }
        //public string CardNumber { get; set; } = default!;
        //public string CardHolderName { get; set; } = default!;
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
