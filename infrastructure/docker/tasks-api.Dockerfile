# Tasks API Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["apps/tasks-api/src/TasksApi.Domain/TasksApi.Domain.csproj", "apps/tasks-api/src/TasksApi.Domain/"]
COPY ["apps/tasks-api/src/TasksApi.Application/TasksApi.Application.csproj", "apps/tasks-api/src/TasksApi.Application/"]
COPY ["apps/tasks-api/src/TasksApi.Infrastructure/TasksApi.Infrastructure.csproj", "apps/tasks-api/src/TasksApi.Infrastructure/"]
COPY ["apps/tasks-api/src/TasksApi.WebApi/TasksApi.WebApi.csproj", "apps/tasks-api/src/TasksApi.WebApi/"]
RUN dotnet restore "apps/tasks-api/src/TasksApi.WebApi/TasksApi.WebApi.csproj"

# Copy everything else and build
COPY apps/tasks-api/src/ apps/tasks-api/src/
WORKDIR "/src/apps/tasks-api/src/TasksApi.WebApi"
RUN dotnet build "TasksApi.WebApi.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TasksApi.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TasksApi.WebApi.dll"]
