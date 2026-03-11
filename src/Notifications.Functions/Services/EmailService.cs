using Microsoft.Extensions.Logging;

namespace Notifications.Functions.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendWelcomeEmailAsync(string fullName, string email)
        {
            var emailBody = $"Olá {fullName}, seja bem-vindo ao FIAP Cloud Games! Seu cadastro foi realizado com sucesso.";
            LogEmail(email, "Bem-vindo!", emailBody, "WelcomeTemplate");
            return Task.CompletedTask;
        }

        public Task SendOrderConfirmationAsync(string userEmail, Guid orderId, decimal amount)
        {
            var emailBody = $"O pagamento do pedido {orderId} no valor de R$ {amount} foi APROVADO. Divirta-se!";
            LogEmail(userEmail, "Compra Confirmada!", emailBody, "OrderConfirmationTemplate");
            return Task.CompletedTask;
        }

        public Task SendGenericEmailAsync(string to, string subject, string body)
        {
            LogEmail(to, subject, body, "GenericMessage");
            return Task.CompletedTask;
        }

        private void LogEmail(string to, string subject, string body, string type)
        {
            _logger.LogInformation("--------------------------------------------------");
            _logger.LogInformation("[EMAIL SERVICE - {Type}] Enviando para: {Email}", type, to);
            _logger.LogInformation("[SUBJECT] {Subject}", subject);
            _logger.LogInformation("[BODY] {Body}", body);
            _logger.LogInformation("--------------------------------------------------");
        }
    }
}
