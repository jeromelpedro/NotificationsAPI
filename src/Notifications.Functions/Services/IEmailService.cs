namespace Notifications.Functions.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string fullName, string email, string correlationId);
        Task SendOrderConfirmationAsync(string userEmail, Guid orderId, decimal amount, string correlationId);
        Task SendGenericEmailAsync(string to, string subject, string body, string correlationId);
    }
}
