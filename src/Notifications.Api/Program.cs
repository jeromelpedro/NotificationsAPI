using Microsoft.ApplicationInsights.Extensibility;
using OpenTelemetry.Trace;
using Notifications.Api.Configurations;
using Notifications.Api.Services;
using Notifications.Api.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((_, services, loggerConfiguration) => loggerConfiguration
	.MinimumLevel.Information()
	.Enrich.FromLogContext()
	.Enrich.With(new Notifications.Api.Serilog.ActivityEnricher())
	.WriteTo.Console()
	.WriteTo.ApplicationInsights(
		services.GetRequiredService<TelemetryConfiguration>(),
		TelemetryConverter.Traces));

builder.Services.AddRabbitMqConfiguration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthConfiguration(builder.Configuration);

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "NotificationsAPI is running on port 5056...");

await app.RunAsync();
