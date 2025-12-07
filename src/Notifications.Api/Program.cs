using Notifications.Api.Models;
using Notifications.Api.Services;
using Notifications.Api.Services.Interfaces;
using Notifications.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenAnyIP(5055);
});

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddHostedService<NotificationConsumerService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "NotificationsAPI is running on port 5055...");

app.Run();
