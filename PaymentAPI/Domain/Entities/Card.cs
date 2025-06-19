using System.ComponentModel.DataAnnotations;

namespace PaymentAPI.Domain.Entities
{
    public class Cards
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public int? CardCVV { get; set; }
        public decimal? CardBalance { get; set; }
        public int? CardExpiryMonth { get; set; }
        public int? CardExpiryYear { get; set; }
        public string IsActive { get; set; } = "Yes";
        public string IsDeleted { get; set; } = "No";
        public DateTime? CreatedOn { get; set; }
        public string CreatedBy { get; set; } = "Anu";
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public ICollection<Transactions> PaymentTransactions { get; set; } = new List<Transactions>();
    }
}
