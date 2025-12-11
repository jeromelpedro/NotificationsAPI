using MassTransit;
using Notifications.Api.Models;
using Notifications.Api.Services.Interfaces;

namespace Notifications.Api.Consumers
{
	public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
	{
		private readonly IEmailService _emailService;

		public PaymentProcessedConsumer(IEmailService emailService)
		{
			_emailService = emailService;
		}

		public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
		{
			var message = context.Message;

			if (message.Status == "Approved")
			{
				await _emailService.SendOrderConfirmationAsync(message.UserEmail, message.OrderId, message.Amount);
			}
		}
	}
}