using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using Notifications.Functions.Telemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddEnvironmentVariables();

// Force minimum logging level and common filters so logs are emitted locally and in Azure
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);

// Ensure ILogger provider forwards logs to Application Insights
builder.Logging.AddApplicationInsights();

builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<ITelemetryInitializer, CorrelationTelemetryInitializer>();

builder.Services.AddSingleton<IEmailService, EmailService>();

await builder.Build().RunAsync();
