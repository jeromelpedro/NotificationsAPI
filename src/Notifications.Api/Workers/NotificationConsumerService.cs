using Microsoft.Extensions.Options;
using Notifications.Api.Models;
using Notifications.Api.Services.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notifications.Api.Workers
{
	public class NotificationConsumerService : BackgroundService
	{
		private readonly ILogger<NotificationConsumerService> _logger;
		private readonly RabbitMqSettings _settings;
		private readonly IServiceProvider _serviceProvider;
		private IConnection? _connection;
		private IModel? _channel;

		// Definição das Routing Keys exatas que sua UsersAPI e PaymentsAPI enviam
		private const string RoutingKeyUserCreated = "UserCreatedEvent";
		private const string RoutingKeyPaymentProcessed = "PaymentProcessedEvent";

		public NotificationConsumerService(IOptions<RabbitMqSettings> options, ILogger<NotificationConsumerService> logger, IServiceProvider serviceProvider)
		{
			_settings = options.Value;
			_logger = logger;
			_serviceProvider = serviceProvider;

			InitializeRabbitMq();
		}

		private void InitializeRabbitMq()
		{
			try
			{
				var factory = new ConnectionFactory
				{
					HostName = _settings.HostName,
					UserName = _settings.UserName,
					Password = _settings.Password,
					Port = _settings.Port,
					DispatchConsumersAsync = true
				};

				_connection = factory.CreateConnection();
				_channel = _connection.CreateModel();

				_channel.ExchangeDeclare(exchange: _settings.ExchangeName, type: ExchangeType.Topic, durable: true);
				_channel.QueueDeclare(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
				_channel.QueueBind(queue: _settings.QueueName, exchange: _settings.ExchangeName, routingKey: RoutingKeyUserCreated);
				_channel.QueueBind(queue: _settings.QueueName, exchange: _settings.ExchangeName, routingKey: RoutingKeyPaymentProcessed);

				_logger.LogInformation("Worker conectado ao Exchange '{Exchange}'. Ouvindo chaves: {Key1}, {Key2}",
					_settings.ExchangeName, RoutingKeyUserCreated, RoutingKeyPaymentProcessed);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro fatal ao configurar RabbitMQ.");
			}
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			if (_channel == null) return Task.CompletedTask;

			var consumer = new AsyncEventingBasicConsumer(_channel);

			consumer.Received += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				var routingKey = ea.RoutingKey;

				try
				{
					await ProcessNotificationAsync(routingKey, message);
					_channel.BasicAck(ea.DeliveryTag, false);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Erro ao processar mensagem: {Message}", message);
				}
			};

			_channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);
			return Task.CompletedTask;
		}

		private async Task ProcessNotificationAsync(string routingKey, string message)
		{
			using var scope = _serviceProvider.CreateScope();
			var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

			switch (routingKey)
			{
				case RoutingKeyUserCreated: 
					var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);
					if (userEvent != null)
					{
						await emailService.SendWelcomeEmailAsync(userEvent.FullName, userEvent.Email);
					}
					break;

				case RoutingKeyPaymentProcessed: 
					var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);
					if (paymentEvent != null && paymentEvent.Status == "Approved")
					{
						await emailService.SendOrderConfirmationAsync(paymentEvent.UserEmail, paymentEvent.OrderId, paymentEvent.Amount);
					}
					break;

				default:
					_logger.LogWarning("Mensagem recebida com Routing Key não mapeada: {RoutingKey}", routingKey);
					break;
			}
		}

		public override void Dispose()
		{
			_channel?.Close();
			_connection?.Close();
			base.Dispose();
		}
	}
}
