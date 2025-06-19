using Microsoft.EntityFrameworkCore;
using PaymentAPI.Domain.Entities;

namespace PaymentAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Cards> Cards { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Card)
                .WithMany(c => c.PaymentTransactions)
                .HasForeignKey(t => t.CardId);
        }
    }
}
