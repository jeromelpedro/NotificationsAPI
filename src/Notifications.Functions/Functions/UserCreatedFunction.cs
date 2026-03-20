using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Helpers;
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
			var correlationId = message.CorrelationIdGetOrCreate();

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
					_logger.LogError(ex,"UserCreatedFunction failed to deserialize message. CorrelationId: {CorrelationId}",correlationId);
					
					await messageActions.AbandonMessageAsync(message);
					return;
				}

				if (mensagem is null)
				{
					_logger.LogWarning("UserCreatedFunction received null payload. CorrelationId: {CorrelationId}",correlationId);
					return;
				}

				_logger.LogInformation("UserCreatedFunction received event for {Email}. CorrelationId: {CorrelationId}",mensagem.Email,correlationId);

				await _emailService.SendWelcomeEmailAsync(mensagem.Nome, mensagem.Email, correlationId);

				await messageActions.CompleteMessageAsync(message);
			}
		}
	}
}