# FIAP Cloud Games - Notifications API

Microsserviço de Notificações (NotificationsAPI): responsável por receber eventos via RabbitMQ e enviar (simulado via logs) e-mails de boas-vindas e de confirmação de compra.

Principais componentes:
- Consumidores MassTransit: `UserCreatedConsumer` (e `PaymentProcessedConsumer` se presente)
- Serviço de envio de e-mail: `EmailService` (realiza logging)

Pré-requisitos
- .NET 9 SDK
- Docker (opcional, para executar em container)
- RabbitMQ (local ou em container)

Executando localmente

1. Restaurar dependências e executar a API:

   dotnet restore
   dotnet run --project src/Notifications.Api/Notifications.Api.csproj

2. A API ficará disponível em http://localhost:5056

Executando com Docker

1. Construir a imagem Docker:

   docker build -t notifications-api -f src/Notifications.Api/Dockerfile .

2. Executar um container da aplicação (exemplo com variáveis de ambiente para RabbitMQ):

   docker run -d --name notifications-api \
     -e ASPNETCORE_ENVIRONMENT=Production \
     -e RabbitMq__HostName=localhost \
     -e RabbitMq__Port=5672 \
     -e RabbitMq__UserName=guest \
     -e RabbitMq__Password=guest \
     -e RabbitMq__ExchangeName=cloudgames.topic \
     -e RabbitMq__QueueNameUserCreated=UserCreatedEvent \
     -e RabbitMq__QueueNamePaymentProcessed=PaymentProcessedEvent \
     -p 5056:5056 notifications-api

Observações sobre RabbitMQ
- Para testar localmente com RabbitMQ você pode rodar o container oficial com management:

  docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

Testes

- O projeto de testes usa xUnit e Moq. Execute:

  dotnet test

Contribuição

- Pull requests são bem-vindos.
