using MassTransit;
using Microsoft.Extensions.Options;
using Notifications.Api.Consumers;
using Notifications.Api.Models;

namespace Notifications.Api.Configurations
{
	public static class MassTransitConfig
	{
		public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));

			services.AddMassTransit(x =>
			{
				x.AddConsumer<UserCreatedConsumer>();
				x.AddConsumer<PaymentProcessedConsumer>();

				x.UsingRabbitMq((context, cfg) =>
				{
					var settings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

					var uri = new Uri($"rabbitmq://{settings.HostName}:{settings.Port}/");

					cfg.Host(uri, h =>
					{
						h.Username(settings.UserName);
						h.Password(settings.Password);
					});

					cfg.Publish<UserCreatedEvent>(p => p.Exclude = true);
					cfg.Publish<PaymentProcessedEvent>(p => p.Exclude = true);

					cfg.ReceiveEndpoint(settings.QueueNameUserCreated, e =>
					{
						e.ConfigureConsumeTopology = false;
						e.Durable = true;
						e.AutoDelete = false;

						e.ClearSerialization();
						e.UseRawJsonSerializer();

						e.ConfigureConsumer<UserCreatedConsumer>(context);

						e.Bind(settings.ExchangeName, s =>
						{
							s.RoutingKey = settings.QueueNameUserCreated;
							s.ExchangeType = "topic";
						});
					});

					cfg.ReceiveEndpoint(settings.QueueNamePaymentProcessed, e =>
					{
						e.ConfigureConsumeTopology = false;
						e.Durable = true;
						e.AutoDelete = false;

						e.ClearSerialization();
						e.UseRawJsonSerializer();

						e.ConfigureConsumer<PaymentProcessedConsumer>(context);

						var routingKey = settings.QueueNamePaymentProcessed.Split('-')[0];
						e.Bind(settings.ExchangeName, s =>
						{
							s.RoutingKey = routingKey;
							s.ExchangeType = "topic";
						});
					});
				});
			});

			return services;
		}
	}
}