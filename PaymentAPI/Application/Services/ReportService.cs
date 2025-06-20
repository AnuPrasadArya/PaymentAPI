using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Data;
using System.Transactions;

namespace PaymentAPI.Application.Services
{
    public class ReportService :IReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ReportService> _logger;
        public ReportService(ApplicationDbContext db, ILogger<ReportService> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<ReportResponse<Transactions>> GetPaymentReport(ReportPaymentRequest request)
        {
            try
            {
                var query = _db.Transactions.AsQueryable();

                if (!string.IsNullOrEmpty(request.CardNumber))
                    query = query.Where(p => p.CardNumber == request.CardNumber);

                if (!string.IsNullOrEmpty(request.ReferenceId))
                    query = query.Where(p => p.ReferenceId == request.ReferenceId);

                if (!string.IsNullOrEmpty(request.Status) &&
                    Enum.TryParse<TransactionStatus>(request.Status, out var status))
                    query = query.Where(p => p.TransactionStatus == Convert.ToString(status));

                if (request.FromDate.HasValue)
                    query = query.Where(p => p.CreatedOn >= request.FromDate.Value);

                if (request.ToDate.HasValue)
                    query = query.Where(p => p.CreatedOn <= request.ToDate.Value);

                var total = await query.CountAsync();
                var items = await query
                    .OrderByDescending(p => p.CreatedOn)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                _logger.LogInformation("Data Fetched Successfully");
                return new ReportResponse<Transactions>
                {
                    Items = items,
                    TotalCount = total,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in Report fetching");                
                throw;
            }
           
        }
        public async Task<ReportResponse<Cards>> GetCardBalanceReport(ReportCardBalanceRequest request)
        {
            try
            {
                var query = _db.Cards.AsQueryable();

                if (!string.IsNullOrEmpty(request.CardHolder))
                    query = query.Where(c => c.CardHolderName.Contains(request.CardHolder));

                if (!string.IsNullOrEmpty(request.CardNumber))
                    query = query.Where(c => c.CardNumber == request.CardNumber);

                var total = await query.CountAsync();
                var items = await query
                    .OrderBy(c => c.CardHolderName)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                _logger.LogInformation("Data Fetched Successfully");
                return new ReportResponse<Cards>
                {
                    Items = items,
                    TotalCount = total,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetCardBalanceReport fetching");
                throw;               
            }
           
        }
    }
}
