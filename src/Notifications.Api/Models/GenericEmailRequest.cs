namespace Notifications.Api.Models
{
	public class GenericEmailRequest
	{
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
