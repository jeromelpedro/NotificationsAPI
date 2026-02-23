using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Monta a connection string a partir dos fragmentos
var config = builder.Configuration;
var host = config["RabbitMq__HostName"];
var port = config["RabbitMq__Port"];
var user = config["RabbitMq__UserName"];
var pass = config["RabbitMq__Password"];
var connectionString = $"amqp://{user}:{pass}@{host}:{port}";

// Injeta como a chave que o trigger espera
builder.Configuration["RabbitMqConnection"] = connectionString;

builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
