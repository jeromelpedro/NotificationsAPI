using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Helpers;
using System.Diagnostics;
using Notifications.Functions.Models;
using Notifications.Functions.Services;
using System.Text.Json;

namespace Notifications.Functions.Functions
{
	public class UserCreatedFunction(IEmailService _emailService, ILogger<UserCreatedFunction> _logger)
	{
		[Function("UserCreatedFunction")]
		public async Task Run(
			[ServiceBusTrigger("%QueueNameUserCreated%", Connection = "AzureServiceBus")]
			ServiceBusReceivedMessage message,
			ServiceBusMessageActions messageActions)
		{			
			var correlationId = message.CorrelationIdGetOrCreate();

			// Propagate correlation to Activity so Application Insights traces include it
			Activity.Current?.SetTag("CorrelationId", correlationId);
			Activity.Current?.AddBaggage("CorrelationId", correlationId);

			using (_logger.BeginScope(new Dictionary<string, object?>
			{
				["CorrelationId"] = correlationId
			}))
			{
				UserCreatedEvent? mensagem = null;

				try
				{
					mensagem = JsonSerializer.Deserialize<UserCreatedEvent>(message.Body);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex,"UserCreatedFunction failed to deserialize message.");
					
					await messageActions.AbandonMessageAsync(message);
					return;
				}

				if (mensagem is null)
				{
					_logger.LogWarning("UserCreatedFunction received null payload.");
					return;
				}

				_logger.LogInformation("UserCreatedFunction received event for {Email}",mensagem.Email);

				await _emailService.SendWelcomeEmailAsync(mensagem.Nome, mensagem.Email);

				await messageActions.CompleteMessageAsync(message);
			}
		}
	}
}