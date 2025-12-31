namespace Notifications.Api.Models
{
	public enum PaymentStatus
	{
		Approved,
		Rejected
	}
	public class PaymentProcessedEvent
	{
		public Guid OrderId { get; set; }
		public Guid UserId { get; set; }
		public string EmailUser { get; set; }
		public decimal Price { get; set; }
		public PaymentStatus Status { get; set; } // "Approved", "Declined"
	}
}
