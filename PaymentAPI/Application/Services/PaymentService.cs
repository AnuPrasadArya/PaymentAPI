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
        public PaymentService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;           
        }
        public async Task<(string ReferenceId, string RefundCode, string Message)> ProcessPayment(string CardNumber, string ReferenceId, decimal Amount)
        {
            var IsExistingReferenceId = await _db.Transactions.FirstOrDefaultAsync(r => r.ReferenceId == ReferenceId);
            if (IsExistingReferenceId != null)
            {
                return (IsExistingReferenceId.ReferenceId!, IsExistingReferenceId.RefundCode!, "ReferenceId Already Exist");

            }
            var IsValidCard = await _db.Cards.FirstOrDefaultAsync(c => c.CardNumber == CardNumber && c.CardBalance > Amount);
            if (IsValidCard == null || IsValidCard.CardBalance < Amount)
            {
                return (ReferenceId, "", "Card Is Not valid or Insufficient Fund");
            }
            string RefundCode =Helper. GenerateRefundcode();
            var transactions = new Transactions
            {
                CardId= IsValidCard.Id,
                ReferenceId = ReferenceId,
                CardNumber = CardNumber,
                TransactionAmount = Amount,
                TransactionStatus = Convert.ToString(PaymentStatus.Held),
                RefundCode = RefundCode,
                RefundCodeExpiry = DateTime.Now.AddDays(1),
                CreatedOn = DateTime.Now
            };

            await _db.Transactions.AddAsync(transactions);
            await _db.SaveChangesAsync();
            return (ReferenceId, RefundCode, "Success");

        }
    }
}
