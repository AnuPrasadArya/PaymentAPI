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
        private readonly ILogger<RefundService> _logger;
        public RefundService(ApplicationDbContext db, ILogger<RefundService> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<(string ReferenceId, string RefundCode, string Message)> ProcessRefund(string ReferenceId, string RefundCode)
        {
            try
            {
                var IsValidRefundRequest = await _db.Transactions.Include(t => t.Card).FirstOrDefaultAsync(t => t.ReferenceId == ReferenceId && t.RefundCode == RefundCode);

                if (IsValidRefundRequest == null)
                {
                    _logger.LogWarning("Invalid ReferenceId or RefundCode - ReferenceId: {ReferenceId}, RefundCode: {RefundCode}", ReferenceId, RefundCode);
                    return (ReferenceId, RefundCode, "Invalid ReferenceId or RefundCode");

                }
                if (IsValidRefundRequest.TransactionStatus != Convert.ToString(PaymentStatus.Held))
                {
                    _logger.LogInformation("Refund not processed", IsValidRefundRequest.TransactionStatus);
                    return (ReferenceId, RefundCode, "Payment already -" + IsValidRefundRequest.TransactionStatus + "");
                }
                DateTime ExpiryDate = Convert.ToDateTime(IsValidRefundRequest.RefundCodeExpiry);
                if (DateTime.Now >= ExpiryDate)
                {
                    _logger.LogInformation("Refund code expired for ReferenceId: {ReferenceId}", ReferenceId);
                    return (ReferenceId, RefundCode, "Refund Code Expired");
                }
                decimal RefundAmount = (decimal)IsValidRefundRequest.TransactionAmount!;
                IsValidRefundRequest.Card.CardBalance += RefundAmount;
                IsValidRefundRequest.TransactionStatus = Convert.ToString(PaymentStatus.Refunded);

                await _db.SaveChangesAsync();
                _logger.LogInformation("Refund processed successfully for ReferenceId: {ReferenceId}", ReferenceId);
                return (ReferenceId, RefundCode, "Refund processed successfully");

            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed for RefId: {ReferenceId}", ReferenceId);
                return (ReferenceId, "", "Database Error Occurred");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in ProcessPayment for RefId: {ReferenceId}", ReferenceId);
                return (ReferenceId, "", "Internal Server Error");
            }

        }
    }

}
