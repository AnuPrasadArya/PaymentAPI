namespace PaymentAPI.Application.Interfaces
{
    public interface ICardValidation
    {
        Task<bool> ValidateCard(string CardNumber, int ExpiryMonth, int ExpiryYear);
    }
}
