namespace PaymentAPI.Application.Interfaces
{
    public interface ICardValidation
    {
        Task<bool> ValidateCard(string CardNumber,int CVV, int ExpiryMonth, int ExpiryYear);
    }
}
