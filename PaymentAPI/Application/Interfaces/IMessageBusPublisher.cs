using PaymentAPI.Domain.Events;

namespace PaymentAPI.Application.Interfaces
{
    public interface IMessageBusPublisher
    {
        void PublishPaymentSuccess(PaymentSuccessEvent paymentEvent);
    }
}
