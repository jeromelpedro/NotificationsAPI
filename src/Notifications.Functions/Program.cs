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

builder.Logging.AddApplicationInsights();
builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<ITelemetryInitializer, CorrelationTelemetryInitializer>();

builder.Services.AddSingleton<IEmailService, EmailService>();

await builder.Build().RunAsync();
