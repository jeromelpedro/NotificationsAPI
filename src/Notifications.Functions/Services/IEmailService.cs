namespace Notifications.Functions.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string fullName, string email);
        Task SendOrderConfirmationAsync(string userEmail, Guid orderId, decimal amount);
        Task SendGenericEmailAsync(string to, string subject, string body);
    }
}
