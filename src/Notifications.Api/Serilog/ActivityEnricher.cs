using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Notifications.Api.Serilog
{
	public class ActivityEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var activity = Activity.Current;
			if (activity == null) return;

			if (activity.TraceId != default)
			{
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
			}

			if (activity.SpanId != default)
			{
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
			}

			if (activity.ParentSpanId != default)
			{
				logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentSpanId", activity.ParentSpanId.ToString()));
			}
		}
	}
}