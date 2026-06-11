# Setup Guide

Use Docker Compose for day-to-day setup. The full BLA Task Management System runs as a containerized stack, so Docker gives you the expected ports, environment variables, seeded data, and persistent storage with the fewest moving parts.

## Recommended: Docker-first setup

### Prerequisites
- Docker Desktop (or Docker Engine + Docker Compose)
- Git

### Start the full stack
```bash
git clone https://github.com/dkennyq/bla-task-management-system.git
cd bla-task-management-system
docker compose up -d
```

### What starts
All core services run in containers:

| Service | Host Port | Notes |
|---|---:|---|
| Web UI | `3000` | Vue frontend |
| Tasks API | `5001` | .NET API + Swagger |
| Users API | `5002` | .NET API + Swagger |
| MongoDB | `27017` | Tasks database |
| PostgreSQL | `5432` | Users database |
| Seq | `8081` | Log dashboard |

Check status with:
```bash
docker compose ps
```

View logs with:
```bash
docker compose logs -f
docker compose logs -f web tasks-api users-api
```

## Access the running system
- Web UI: http://localhost:3000
- Tasks API Swagger: http://localhost:5001/swagger
- Users API Swagger: http://localhost:5002/swagger
- Seq: http://localhost:8081

### Login credentials
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

## Environment variables in `docker-compose.yml`
The Docker stack already defines the important runtime configuration.

### `tasks-api`
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080`
- `MongoDB__ConnectionString=mongodb://mongodb:27017`
- `MongoDB__DatabaseName=tasksdb`
- `Jwt__Secret=...`
- `Jwt__Issuer=TaskManagementSystem`
- `Jwt__Audience=TaskManagementApp`
- `Jwt__ExpirationMinutes=60`
- `CORS__AllowedOrigins__0=http://localhost:3000`
- `Serilog__WriteTo__1__Args__serverUrl=http://seq:5341`

### `users-api`
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080`
- `Postgres__ConnectionString=Host=postgres;Port=5432;Database=usersdb;Username=admin;Password=admin123;Include Error Detail=true`
- `Jwt__Secret=...`
- `Jwt__Issuer=TaskManagementSystem`
- `Jwt__Audience=TaskManagementApp`
- `Jwt__ExpirationMinutes=60`
- `CORS__AllowedOrigins__0=http://localhost:3000`
- `Serilog__WriteTo__1__Args__serverUrl=http://seq:5341`

### `web`
- `VITE_TASKS_API_URL=http://localhost:5001/api`
- `VITE_USERS_API_URL=http://localhost:5002/api`

### infrastructure services
- `mongodb`: `MONGO_INITDB_DATABASE=tasksdb`
- `postgres`: `POSTGRES_DB=usersdb`, `POSTGRES_USER=admin`, `POSTGRES_PASSWORD=admin123`
- `seq`: `ACCEPT_EULA=Y`, `SEQ_FIRSTRUN_NOAUTHENTICATION=true`

## Data persistence
Docker Compose uses named volumes for MongoDB, PostgreSQL, and Seq.

```bash
docker compose down
```
Stops and removes containers but **keeps data volumes**. Your database data remains available the next time you run `docker compose up -d`.

```bash
docker compose down -v
```
Stops containers **and removes volumes**. Use this when you want a clean reset of MongoDB, PostgreSQL, and Seq data.

## Common Docker workflow
```bash
# Rebuild after Dockerfile or dependency changes
docker compose up -d --build

# Restart one service
docker compose restart web

# Inspect a single service log
docker compose logs -f users-api
```

## Brief local development option
Local development without Docker is optional and mainly useful when you need to debug a single service directly.

Minimum tools:
- .NET 8 SDK
- Node.js 20+
- MongoDB 7+
- PostgreSQL 16+

Typical local ports differ from the Docker stack, so if you run services outside containers you must supply your own app settings, database connections, and frontend `VITE_*` variables.

For most contributors, use Docker for the full stack and only run an individual service locally when necessary.

## Troubleshooting
### A port is already in use
Stop the conflicting process or change the published port in `docker-compose.yml`.

### A service is not healthy
```bash
docker compose ps
docker compose logs mongodb postgres tasks-api users-api
```

### You want a clean environment
```bash
docker compose down -v
docker compose up -d --build
```

[<- Back to README](../README.md)
