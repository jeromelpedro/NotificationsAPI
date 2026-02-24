using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddEnvironmentVariables();

var rabbitHost = builder.Configuration["RabbitMq:HostName"];
var rabbitPort = builder.Configuration["RabbitMq:Port"];
var rabbitUser = builder.Configuration["RabbitMq:UserName"];
var rabbitPass = builder.Configuration["RabbitMq:Password"];

var connectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}";
builder.Configuration["RabbitMqConnection"] = connectionString;

builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
