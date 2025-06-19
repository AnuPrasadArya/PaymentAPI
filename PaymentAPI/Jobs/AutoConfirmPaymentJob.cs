using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Entities;
using PaymentAPI.Infrastructure.Data;
using Quartz;

namespace PaymentAPI.Jobs
{
    public class AutoConfirmPaymentJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public AutoConfirmPaymentJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var now = DateTime.Now;

            var paymentsToConfirm = await dbContext.Transactions
                .Where(p => p.TransactionStatus == Convert.ToString(PaymentStatus.Held) && p.RefundCodeExpiry >= now)
                .ToListAsync();

            foreach (var payment in paymentsToConfirm)
            {
                payment.TransactionStatus = Convert.ToString(PaymentStatus.Confirmed);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
