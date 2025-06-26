using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Data;
using PaymentAPI.Helpers;
using System.Transactions;

namespace PaymentAPI.Application.Services
{

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(ApplicationDbContext db, ILogger<PaymentService> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<(string ReferenceId, string RefundCode, string Message)> ProcessPayment(string CardNumber, string ReferenceId, decimal Amount)
        {

            var IsExistingReferenceId = await _db.Transactions.FirstOrDefaultAsync(r => r.ReferenceId == ReferenceId);
            if (IsExistingReferenceId != null)
            {
                _logger.LogWarning("ReferenceId Already Exist: {ReferenceId}", ReferenceId);
                return (IsExistingReferenceId.ReferenceId!, IsExistingReferenceId.RefundCode!, "ReferenceId Already Exist");

            }
            var IsValidCard = await _db.Cards.FirstOrDefaultAsync(c => c.CardNumber == CardNumber && c.CardBalance > Amount);
            if (IsValidCard == null || IsValidCard.CardBalance < Amount)
            {
                _logger.LogWarning("Invalid or insufficient funds for card ");
                return (ReferenceId, "", "Card Is Not valid or Insufficient Fund");
            }
            string RefundCode = Helper.GenerateRefundcode();
            string EncryptedCardNumber = Helper.Encrypt(CardNumber);
            var transactions = new Transactions
            {
                CardId = IsValidCard.Id,
                ReferenceId = ReferenceId,
                CardNumber = EncryptedCardNumber,
                TransactionAmount = Amount,
                TransactionStatus = Convert.ToString(PaymentStatus.Held),
                RefundCode = RefundCode,
                RefundCodeExpiry = DateTime.Now.AddDays(1),
                CreatedOn = DateTime.Now
            };

            await _db.Transactions.AddAsync(transactions);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Payment held successfully. RefId: {ReferenceId}, RefundCode: {RefundCode}", ReferenceId, RefundCode);
            return (ReferenceId, RefundCode, "Success");
        }
    }
}
