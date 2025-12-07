FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5056

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Ajuste do caminho conforme sua imagem: src/Notifications.Api
COPY ["src/Notifications.Api/Notifications.Api.csproj", "src/Notifications.Api/"]

RUN dotnet restore "./src/Notifications.Api/Notifications.Api.csproj"

COPY . .
WORKDIR "/src/src/Notifications.Api"
RUN dotnet build "./Notifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notifications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.Api.dll"]
