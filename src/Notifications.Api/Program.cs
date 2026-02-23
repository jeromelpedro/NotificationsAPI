using Notifications.Api.Configurations;
using Notifications.Api.Services;
using Notifications.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "NotificationsAPI is running on port 5056...");

await app.RunAsync();
