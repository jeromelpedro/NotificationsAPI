using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Models;
using Notifications.Functions.Services;
using System.Text.Json;

namespace Notifications.Functions.Functions
{
    public class UserCreatedFunction(IEmailService _emailService, IConfiguration configuration, ILogger<UserCreatedFunction> _logger)
    {
        [Function("UserCreatedFunction")]
		public async Task Run(
			[ServiceBusTrigger("%QueueNameUserCreated%", Connection = "AzureServiceBus")]
			ServiceBusReceivedMessage message,
			ServiceBusMessageActions messageActions)
		{
			var mensagem = JsonSerializer.Deserialize<UserCreatedEvent>(message.Body);

			if (mensagem is null)
            {
                _logger.LogWarning("UserCreatedFunction received null payload");
                return;
            }

            _logger.LogInformation("UserCreatedFunction received event for {Email}", mensagem.Email);

            await _emailService.SendWelcomeEmailAsync(mensagem.Nome, mensagem.Email);

			await messageActions.CompleteMessageAsync(message);
		}
    }
}