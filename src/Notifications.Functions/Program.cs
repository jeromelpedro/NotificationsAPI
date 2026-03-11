using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Services;
using RabbitMQ.Client;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration.AddEnvironmentVariables();

var rabbitHost = builder.Configuration["RabbitMq:HostName"];
var rabbitPort = builder.Configuration["RabbitMq:Port"];
var rabbitUser = builder.Configuration["RabbitMq:UserName"];
var rabbitPass = builder.Configuration["RabbitMq:Password"];
var queueUserCreated = builder.Configuration["RabbitMq:QueueNameUserCreated"];
var queuePaymentProcessed = builder.Configuration["RabbitMq:QueueNamePaymentProcessed"];
var exchangeName = builder.Configuration["RabbitMq:ExchangeName"];
var routingKey = "PaymentProcessedEvent";

var connectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}";
builder.Configuration["RabbitMqConnection"] = connectionString;

var factory = new ConnectionFactory
{
    HostName = rabbitHost,
    Port = int.Parse(rabbitPort!),
    UserName = rabbitUser,
    Password = rabbitPass
};

try
{
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true);
    await channel.QueueDeclareAsync(queuePaymentProcessed, durable: true, exclusive: false, autoDelete: false);
    await channel.QueueBindAsync(queue: queuePaymentProcessed, exchange: exchangeName, routingKey: routingKey);

    await channel.QueueDeclareAsync(queueUserCreated, durable: true, exclusive: false, autoDelete: false);

    Console.WriteLine("RabbitMQ queues ensured.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error ensuring queues: {ex.Message}");
}

builder.Services
	.AddApplicationInsightsTelemetryWorkerService()
	.ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
