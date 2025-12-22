using MassTransit;
using Moq;
using Notifications.Api.Consumers;
using Notifications.Api.Models;
using Notifications.Api.Services.Interfaces;

namespace Notifications.Tests
{
    public class UserCreatedConsumerTests
    {
        [Fact]
        public async Task Consume_Should_Call_SendWelcomeEmailAsync()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var consumer = new UserCreatedConsumer(mockEmailService.Object);

            var @event = new UserCreatedEvent
            {
                Id = Guid.NewGuid(),
                Nome = "Joao Silva",
                Email = "joao@example.com"
            };

            var mockContext = new Mock<ConsumeContext<UserCreatedEvent>>();
            mockContext.Setup(c => c.Message).Returns(@event);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockEmailService.Verify(s => s.SendWelcomeEmailAsync(@event.Nome, @event.Email), Times.Once);
        }

        [Fact]
        public async Task Consume_When_EmailService_Throws_Should_PropagateException()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var consumer = new UserCreatedConsumer(mockEmailService.Object);

            var @event = new UserCreatedEvent
            {
                Id = Guid.NewGuid(),
                Nome = "Carlos",
                Email = "carlos@example.com"
            };

            var mockContext = new Mock<ConsumeContext<UserCreatedEvent>>();
            mockContext.Setup(c => c.Message).Returns(@event);

            mockEmailService
                .Setup(s => s.SendWelcomeEmailAsync(@event.Nome, @event.Email))
                .ThrowsAsync(new InvalidOperationException("SMTP failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.Consume(mockContext.Object));
        }
    }
}
