using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Models;
using Notifications.Functions.Services;
using System.Text.Json;

namespace Notifications.Functions.Functions
{
	public class UserCreatedFunction
	{
		private readonly IEmailService _emailService;
		private readonly ILogger<UserCreatedFunction> _logger;

		public UserCreatedFunction(IEmailService emailService, ILogger<UserCreatedFunction> logger)
		{
			_emailService = emailService;
			_logger = logger;
		}

		[Function("UserCreatedFunction")]
		public async Task Run([
			RabbitMQTrigger("%RabbitMq:QueueNameUserCreated%", ConnectionStringSetting = "RabbitMqConnection")] byte[] body)
		{
			var payload = System.Text.Encoding.UTF8.GetString(body);
			var @event = JsonSerializer.Deserialize<UserCreatedEvent>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (@event is null)
			{
				_logger.LogWarning("UserCreatedFunction received invalid payload");
				return;
			}

			_logger.LogInformation("UserCreatedFunction received event for {Email}", @event.Email);
			await _emailService.SendWelcomeEmailAsync(@event.Nome, @event.Email);
		}
	}
}
