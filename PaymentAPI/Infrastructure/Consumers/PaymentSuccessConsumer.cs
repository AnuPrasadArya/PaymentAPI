using PaymentAPI.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentAPI.Infrastructure.Consumers
{
    public class PaymentSuccessConsumer : BackgroundService
    {
        private readonly ILogger<PaymentSuccessConsumer> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public PaymentSuccessConsumer(ILogger<PaymentSuccessConsumer> logger)
        {
            _logger = logger;
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672, // ✅ correct port
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "payment_success_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation("✅ RabbitMQ connection and queue initialized.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel!);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var paymentEvent = JsonSerializer.Deserialize<PaymentSuccessEvent>(message);

                    if (paymentEvent != null)
                    {
                        _logger.LogInformation("📥 Payment event received: {Name} paid {Amount} at {Time}",
                            paymentEvent.CustomerName, paymentEvent.Amount, DateTime.Now.ToString());

                        // TODO: Add real handling logic (email, DB, etc.)
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed to process payment event.");
                }
            };

            _channel!.BasicConsume(
                queue: "payment_success_queue",
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
