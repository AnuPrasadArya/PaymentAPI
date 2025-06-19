using System.Text.Json.Serialization;

namespace PaymentAPI.Domain.Entities
{
    public class Transactions
    {

        public int Id { get; set; }
        public string? ReferenceId { get; set; }
        public string? CardNumber { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string? TransactionStatus { get; set; }
        public string? TransactionDevice { get; set; } = "System";
        public string? RefundCode { get; set; }
        public DateTime? RefundCodeExpiry { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string IsDeleted { get; set; } = "No";
        public int CardId { get; set; }
        [JsonIgnore]
        public Cards Card { get; set; } = null!;

    }
    
    public enum PaymentStatus
    {
        Held, Confirmed, Refunded
    }
}

