using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddEnvironmentVariables();

var rabbitHost = Environment.GetEnvironmentVariable("RabbitMq__HostName");
var rabbitPort = Environment.GetEnvironmentVariable("RabbitMq__Port");
var rabbitUser = Environment.GetEnvironmentVariable("RabbitMq__UserName");
var rabbitPass = Environment.GetEnvironmentVariable("RabbitMq__Password");

var connectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}";
builder.Configuration["RabbitMqConnection"] = connectionString;

Console.WriteLine($"[DEBUG] RabbitMQ connection string: {connectionString}");


builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
