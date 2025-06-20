using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Helpers;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.Application.Services
{
    public class CardService : ICardValidation
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CardService> _logger;
        public CardService(ApplicationDbContext db, ILogger<CardService> logger)
        {
            _db = db;
            _logger = logger;

        }
        public async Task<bool> ValidateCard(string CardNumber,int CVV, int ExpiryMonth, int ExpiryYear)
        {
            try
            {
                string EncryptedCardNumber = Helper.Encrypt(CardNumber);
                var IsValidCard = await _db.Cards.FirstOrDefaultAsync(c => c.CardNumber == EncryptedCardNumber && c.CardCVV == CVV && c.CardExpiryMonth == ExpiryMonth && c.CardExpiryYear == ExpiryYear);
                _logger.LogInformation("Fetch Card details successfully for CardNumber: {CardNumber}", CardNumber);

                return IsValidCard != null;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database Fetch failed for CardNumber: {CardNumber}", CardNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in ValidateCard for CardNumber: {CardNumber}", CardNumber);
                return false;
            }

        }
    }
}
