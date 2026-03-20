using Microsoft.Extensions.Logging;

namespace Notifications.Functions.Services
{
    public class EmailService(ILogger<EmailService> _logger) : IEmailService
    {
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
			_logger.LogInformation(
				"--------------------------------------------------\n" +
				"[EMAIL SERVICE - {Type}]\n" +
				"  CorrelationId : {CorrelationId}\n" +
				"  To            : {Email}\n" +
				"  Subject       : {Subject}\n" +
				"  Body          : {Body}\n" +
				"--------------------------------------------------",
				type, to, subject, body);			
		}
    }
}
