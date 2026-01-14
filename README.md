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

## ☸️ Kubernetes

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) com Kubernetes habilitado
- [kubectl](https://kubernetes.io/docs/tasks/tools/) (já incluso no Docker Desktop)

### Habilitar Kubernetes no Docker Desktop

1. Abra o **Docker Desktop**
2. Vá em **Settings** (ícone de engrenagem)
3. Clique em **Kubernetes** no menu lateral
4. Marque **Enable Kubernetes**
5. Clique em **Apply & Restart**
6. Aguarde o Kubernetes iniciar (ícone verde no canto inferior esquerdo)

### Deploy da Aplicação

#### Passo 1: Construir a imagem Docker

```bash
# Na raiz do projeto
docker build -t notifications-api:latest .
```

#### Passo 2: Aplicar os manifests Kubernetes

```bash
# Aplicar todos os recursos (ConfigMap, Secret, Deployment e Service)
kubectl apply -f ./k8s/
```

**Saída esperada:**
```
configmap/notifications-api-config created
deployment.apps/notifications-api created
secret/notifications-api-secret created
service/notifications-api created
```

#### Passo 3: Verificar o status

```bash
# Ver status dos pods
kubectl get pods

# Ver status dos serviços
kubectl get services

# Ver logs da aplicação
kubectl logs -f deployment/notifications-api
```

**Saída esperada:**
```
NAME                           READY   STATUS    RESTARTS   AGE
notifications-api-75b78fc9f-xxxxx   1/1     Running   0          30s
```

#### Passo 4: Acessar a aplicação

Como o Service é do tipo `ClusterIP`, use **port-forward** para acessar localmente:

```bash
kubectl port-forward service/notifications-api 5056:5056
```

A aplicação estará disponível em:
- **API:** http://localhost:5056
- **Swagger:** http://localhost:5056/swagger

### Arquivos de Configuração Kubernetes

| Arquivo | Descrição |
|---------|-----------|
| `k8s/configmap.yaml` | Configurações não-sensíveis (hostname RabbitMQ, filas, etc.) |
| `k8s/secret.yaml` | Credenciais sensíveis (usuário/senha RabbitMQ em Base64) |
| `k8s/deployment.yaml` | Definição do pod, replicas, health checks e recursos |
| `k8s/service.yaml` | Exposição do serviço internamente no cluster |

### Comandos Úteis

```bash
# Ver detalhes do pod
kubectl describe pod -l app=notifications-api

# Ver eventos do cluster
kubectl get events --sort-by='.lastTimestamp'

# Escalar replicas
kubectl scale deployment/notifications-api --replicas=3

# Atualizar após mudanças na imagem
docker build -t notifications-api:latest .
kubectl rollout restart deployment/notifications-api

# Remover todos os recursos
kubectl delete -f ./k8s/
```

### Troubleshooting

| Problema | Solução |
|----------|---------|
| Pod em `CrashLoopBackOff` | Verifique logs: `kubectl logs deployment/notifications-api` |
| Pod em `Pending` | Verifique recursos: `kubectl describe pod -l app=notifications-api` |
| Conexão recusada | Verifique se o port-forward está ativo |
| RabbitMQ não conecta | O RabbitMQ precisa estar rodando no cluster ou acessível via hostname configurado |

> ⚠️ **Nota:** A aplicação depende do RabbitMQ. Para um ambiente Kubernetes completo, você precisará fazer deploy do RabbitMQ no cluster ou configurar o `configmap.yaml` para apontar para um RabbitMQ externo.