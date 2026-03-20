using Azure.Messaging.ServiceBus;

namespace Notifications.Functions.Helpers
{
	public static class CorrelationIdHelper
	{
		public static string CorrelationIdGetOrCreate(this ServiceBusReceivedMessage message)
		{
			if (!string.IsNullOrWhiteSpace(message.CorrelationId))
				return message.CorrelationId;

			if (message.ApplicationProperties.TryGetValue("CorrelationId", out var propValue)
				&& propValue is string s
				&& !string.IsNullOrWhiteSpace(s))
				return s;

			return Guid.NewGuid().ToString();
		}
	}
}
