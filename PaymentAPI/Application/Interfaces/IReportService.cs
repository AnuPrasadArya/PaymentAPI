using PaymentAPI.Application.DTOs;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Application.Interfaces
{
    public interface IReportService
    {
        Task<ReportResponse<Transactions>> GetPaymentReport(ReportPaymentRequest request);
        Task<ReportResponse<Cards>> GetCardBalanceReport(ReportCardBalanceRequest request);
    }
}
