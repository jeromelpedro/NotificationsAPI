using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Models;
using Notifications.Functions.Services;
using System.Text.Json;

namespace Notifications.Functions.Functions
{
	public class PaymentProcessedFunction
	{
		private readonly IEmailService _emailService;
		private readonly ILogger<PaymentProcessedFunction> _logger;

		public PaymentProcessedFunction(IEmailService emailService, ILogger<PaymentProcessedFunction> logger)
		{
			_emailService = emailService;
			_logger = logger;
		}

		[Function("PaymentProcessedFunction")]
		public async Task Run([
			RabbitMQTrigger("%RabbitMq:QueueNamePaymentProcessed%", ConnectionStringSetting = "RabbitMqConnection")] byte[] body)
		{
			var payload = System.Text.Encoding.UTF8.GetString(body);
			var @event = JsonSerializer.Deserialize<PaymentProcessedEvent>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (@event is null)
			{
				_logger.LogWarning("PaymentProcessedFunction received invalid payload");
				return;
			}

			_logger.LogInformation("PaymentProcessedFunction received event for order {OrderId}", @event.OrderId);

			if (@event.Status == PaymentStatus.Approved)
			{
				await _emailService.SendOrderConfirmationAsync(@event.EmailUser, @event.OrderId, @event.Price);
			}
		}
	}
}
