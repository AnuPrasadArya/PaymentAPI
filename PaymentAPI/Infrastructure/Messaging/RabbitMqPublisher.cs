using PaymentAPI.Application.Interfaces;
using PaymentAPI.Domain.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PaymentAPI.Infrastructure.Messaging
{
    public class RabbitMqPublisher : IMessageBusPublisher
    {
        private readonly IConfiguration _config;

        public RabbitMqPublisher(IConfiguration config)
        {
            _config = config;
        }

        public void PublishPaymentSuccess(PaymentSuccessEvent paymentEvent)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"] ?? "localhost",
                UserName = _config["RabbitMQ:UserName"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "payment_success_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(paymentEvent));

            channel.BasicPublish(exchange: "", routingKey: "payment_success_queue", body: body);
        }
    }
}
