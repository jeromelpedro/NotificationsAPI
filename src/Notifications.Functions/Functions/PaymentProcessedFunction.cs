using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Models;
using Notifications.Functions.Services;
using System.Text.Json;

namespace Notifications.Functions.Functions
{
    public class PaymentProcessedFunction(IEmailService _emailService, ILogger<PaymentProcessedFunction> _logger)
    {
        [Function("PaymentProcessedFunction")]
        public async Task Run(
			[ServiceBusTrigger("%QueueNamePaymentProcessed%", "%SubscriptionPaymentProcessed%", Connection = "AzureServiceBus")]
			ServiceBusReceivedMessage message,
			ServiceBusMessageActions messageActions)
        {

			var mensagem = JsonSerializer.Deserialize<PaymentProcessedEvent>(message.Body);


            if (mensagem is null)
            {
                _logger.LogWarning("PaymentProcessedFunction received null payload");
                return;
            }

            _logger.LogInformation("PaymentProcessedFunction received event for order {OrderId}", mensagem.OrderId);

            if (mensagem.Status == PaymentStatus.Approved)
            {
                await _emailService.SendOrderConfirmationAsync(
					mensagem.EmailUser,
					mensagem.OrderId,
					mensagem.Price);
            }

			await messageActions.CompleteMessageAsync(message);
		}
    }
}