# Users API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["apps/users-api/src/UsersApi.Domain/UsersApi.Domain.csproj", "apps/users-api/src/UsersApi.Domain/"]
COPY ["apps/users-api/src/UsersApi.Application/UsersApi.Application.csproj", "apps/users-api/src/UsersApi.Application/"]
COPY ["apps/users-api/src/UsersApi.Infrastructure/UsersApi.Infrastructure.csproj", "apps/users-api/src/UsersApi.Infrastructure/"]
COPY ["apps/users-api/src/UsersApi.WebApi/UsersApi.WebApi.csproj", "apps/users-api/src/UsersApi.WebApi/"]
RUN dotnet restore "apps/users-api/src/UsersApi.WebApi/UsersApi.WebApi.csproj"

# Copy everything else and build
COPY apps/users-api/src/ apps/users-api/src/
WORKDIR "/src/apps/users-api/src/UsersApi.WebApi"
RUN dotnet build "UsersApi.WebApi.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "UsersApi.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersApi.WebApi.dll"]
