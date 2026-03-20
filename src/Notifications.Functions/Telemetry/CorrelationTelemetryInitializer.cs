using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics;

namespace Notifications.Functions.Telemetry
{
    public class CorrelationTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            try
            {
                string? correlationId = null;

                var activity = Activity.Current;
                if (activity != null)
                {
                    // Check tags first
                    var tag = activity.GetTagItem("CorrelationId") as string;
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        correlationId = tag;
                    }

                    // Also check baggage
                    if (string.IsNullOrWhiteSpace(correlationId) && activity.Baggage != null)
                    {
                        foreach (var b in activity.Baggage)
                        {
                            if (string.Equals(b.Key, "CorrelationId", StringComparison.OrdinalIgnoreCase))
                            {
                                correlationId = b.Value;
                                break;
                            }
                        }
                    }
                }

                // If not found on Activity, try telemetry properties
                if (string.IsNullOrWhiteSpace(correlationId) && telemetry.Context?.Properties != null)
                {
                    if (telemetry.Context.Properties.TryGetValue("CorrelationId", out var v) && !string.IsNullOrWhiteSpace(v))
                    {
                        correlationId = v;
                    }
                }

                if (!string.IsNullOrWhiteSpace(correlationId))
                {
                    // Add to custom properties
                    telemetry.Context.Properties["CorrelationId"] = correlationId;

                    // Optionally map to Operation.Id so traces/requests share the same operation id
                    telemetry.Context.Operation.Id = correlationId;
                }
            }
            catch
            {
                // Swallow any exceptions to not break telemetry
            }
        }
    }
}
