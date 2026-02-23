using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
// Garante que a configuração está completamente carregada antes de montar a string
builder.Configuration.AddEnvironmentVariables();

var config = builder.Configuration;
var rabbitHost = config["RabbitMq__HostName"];
var rabbitPort = config["RabbitMq__Port"] ?? "5672";
var rabbitUser = config["RabbitMq__UserName"];
var rabbitPass = config["RabbitMq__Password"];

if (!string.IsNullOrEmpty(rabbitHost))
{
	var connectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}";
	builder.Configuration["RabbitMqConnection"] = connectionString;
}

builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
