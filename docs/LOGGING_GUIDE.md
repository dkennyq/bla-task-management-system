# Backend Logging - Serilog + Seq

**Issue**: [#21 - Backend Logging: Implement Serilog with Seq](https://github.com/dkennyq/bla-task-management-system/issues/21)  
**Priority**: Medium  
**Created**: 2026-06-10

---

## 🎯 Architecture

```
┌──────────────────┐    Serilog Sink (HTTP)    ┌──────────────────┐
│   Tasks API      │ ──────────────────────────▶│                  │
│   Port 5001      │                            │    Seq Server    │
├──────────────────┤                            │   Port 5341     │
│   Users API      │ ──────────────────────────▶│   (Docker)      │
│   Port 5002      │                            │                  │
└──────────────────┘                            │  Dashboard UI   │
                                                │  REST API       │
┌──────────────────┐                            └──────────────────┘
│   Frontend       │
│   Port 3000      │
└──────────────────┘
```

## 📁 File Changes

### Docker Compose
```yaml
# docker-compose.yml (new service)
seq:
  image: datalust/seq:latest
  container_name: seq
  ports:
    - "5341:5341"       # UI + ingestion HTTP
    - "5342:5342"       # Ingestion HTTPS (optional)
  environment:
    ACCEPT_EULA: "Y"
  volumes:
    - seq-data:/data
  networks:
    - app-network
```

### Both APIs - NuGet Packages
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Seq
```

### Both APIs - Program.cs Changes
```csharp
using Serilog;

// Before builder
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// After app runs (cleanup)
try { app.Run(); }
finally { Log.CloseAndFlush(); }
```

### Both APIs - appsettings.json
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "System": "Warning",
      "MongoDB": "Information"
    }
  },
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "Seq",
      "Args": { "serverUrl": "http://seq:5341" }
    }
  ],
  "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
}
```

## 🚀 Running

```bash
# Start everything (includes Seq)
docker-compose up -d

# Seq UI available at:
http://localhost:5341

# To see app logs in Seq:
#   - Open http://localhost:5341
#   - Filter: @Level = "Error"
#   - Filter: Application = "TasksApi"
#   - Filter: Application = "UsersApi"
```

## 📊 Seq Dashboard

Seq provides:
- **Real-time log streaming**
- **Structured search** (filter by level, namespace, custom properties)
- **Signal-based alerting**
- **Dashboard with charts** (error rate, request count, etc.)
- **REST API** for programmatic access

### Useful Queries in Seq
```sql
-- All errors in last hour
@Level = "Error" and @Timestamp > now() - 1:00:00

-- Slow requests (>500ms)
RequestDuration > 500

-- Failed logins
@Message like "Failed login*"

-- By specific user
UserId = "00000000-0000-0000-0000-000000000001"
```

## 🔧 Configuration via Environment Variables

```yaml
# docker-compose.yml (per API service)
environment:
  - Serilog__WriteTo__0__Name=Console
  - Serilog__WriteTo__1__Name=Seq
  - Serilog__WriteTo__1__Args__serverUrl=http://seq:5341
  - Serilog__MinimumLevel__Default=Information
  - Serilog__MinimumLevel__Override__Microsoft=Warning
```

## 📝 Logged Events

| Event | Level | Properties |
|-------|-------|------------|
| Request completed | Information | Method, Path, StatusCode, Duration |
| Task created | Information | TaskId, UserId, Title |
| Task updated | Information | TaskId, UserId, Changes |
| Task deleted | Information | TaskId, UserId |
| Login success | Information | Email, UserId |
| Login failed | Warning | Email, Reason |
| Registration | Information | Email, UserId |
| Unauthorized access | Warning | Path, IP |
| Unhandled exception | Error | Full stack trace, Path, UserId |
| DB query timeout | Error | Query, Duration |
| App started | Information | Version, Environment |
| App stopping | Information | - |

## 🔗 Related

- Issue #21: Backend Logging (this issue)
- Issue #19: JWT Authentication
- [Seq Documentation](https://docs.datalust.co/docs)
- [Serilog Documentation](https://serilog.net/)
