namespace Notifications.Functions.Models
{
    public class UserCreatedEvent
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
