using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Data;
using System.Transactions;

namespace PaymentAPI.Application.Services
{
    public class RefundService : IRefundService
    {
        private readonly ApplicationDbContext _db;
        public RefundService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
        }
        public async Task<(string ReferenceId, string RefundCode, string Message)> ProcessRefund(string ReferenceId, string RefundCode)
        {
            var IsValidRefundRequest = await _db.Transactions.Include(t => t.Card).FirstOrDefaultAsync(t => t.ReferenceId == ReferenceId && t.RefundCode == RefundCode);

            if (IsValidRefundRequest == null)
            {
                return (ReferenceId, RefundCode, "Invalid ReferenceId or RefundCode");

            }
            if (IsValidRefundRequest.TransactionStatus != Convert.ToString(PaymentStatus.Held))
            {
                return (ReferenceId, RefundCode, "Payment already -" + IsValidRefundRequest.TransactionStatus + "");
            }
            DateTime ExpiryDate = Convert.ToDateTime(IsValidRefundRequest.RefundCodeExpiry);
            if (DateTime.Now >= ExpiryDate)
            {
                return (ReferenceId, RefundCode, "Refund Code Expired");
            }
            decimal RefundAmount = (decimal)IsValidRefundRequest.TransactionAmount!;
            IsValidRefundRequest.Card.CardBalance += RefundAmount;
            IsValidRefundRequest.TransactionStatus = Convert.ToString(PaymentStatus.Refunded);

            await _db.SaveChangesAsync();
            return (ReferenceId, RefundCode, "Refund processed successfully");

        }
    }

}
