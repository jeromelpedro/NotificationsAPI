using MassTransit;
using Notifications.Api.Models;
using Notifications.Api.Services.Interfaces;

namespace Notifications.Api.Consumers
{
	public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
	{
		private readonly IEmailService _emailService;

		public UserCreatedConsumer(IEmailService emailService)
		{
			_emailService = emailService;
		}

		public async Task Consume(ConsumeContext<UserCreatedEvent> context)
		{
			var message = context.Message;
			await _emailService.SendWelcomeEmailAsync(message.Nome, message.Email);
		}
	}
}