using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Models;
using Notifications.Functions.Services;

namespace Notifications.Functions.Functions
{
    public class UserCreatedFunction(IEmailService _emailService, ILogger<UserCreatedFunction> _logger)
    {
        [Function("UserCreatedFunction")]
        public async Task Run(
            [RabbitMQTrigger("%RabbitMq:QueueNameUserCreated%", ConnectionStringSetting = "RabbitMqConnection")] UserCreatedEvent @event)
        {
            if (@event is null)
            {
                _logger.LogWarning("UserCreatedFunction received null payload");
                return;
            }

            _logger.LogInformation("UserCreatedFunction received event for {Email}", @event.Email);

            await _emailService.SendWelcomeEmailAsync(@event.Nome, @event.Email);
        }
    }
}