# 🔄 Development Workflows - BLA Task Management System

This document explains the two main development workflows and when to use each one.

---

## 📋 Table of Contents

1. [Current Setup](#current-setup)
2. [Workflow 1: Local Development (Recommended)](#workflow-1-local-development-recommended)
3. [Workflow 2: Everything in Docker](#workflow-2-everything-in-docker)
4. [Comparison](#comparison)
5. [When to Use Each One](#when-to-use-each-one)
6. [Useful Commands](#useful-commands)

---

## 🎯 Current Setup

**Current project state:**

```
┌─────────────────────────────────────────────────────┐
│  🐳 IN DOCKER:                                      │
│     • MongoDB (port 27017)                          │
│     • PostgreSQL (port 5432)                        │
│                                                      │
│  🖥️  ON YOUR MACHINE (local):                      │
│     • Tasks API (dotnet run - port 5077)           │
│     • Background process (detached)                 │
│                                                      │
│  🔲 NOT STARTED:                                    │
│     • Users API                                      │
│     • Frontend (Vue.js)                             │
└─────────────────────────────────────────────────────┘
```

**Verify status:**

```powershell
# See .NET processes
Get-Process -Name dotnet

# See Docker containers
docker ps

# Test API
curl http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001
```

---

## 🏗️ Workflow 1: Local Development (Recommended)

### ✅ Advantages

- ⚡ **Hot Reload**: Changes apply automatically without restart
- 🐛 **Full debugging**: Breakpoints, variable inspection
- 🚀 **Fast**: No Docker image rebuilds
- 🧪 **TDD-friendly**: Tests run instantly
- 📝 **Clear logs**: Direct console output

### 📝 How It Works

**Services:**
- ✅ **Databases**: In Docker (MongoDB + PostgreSQL)
- ✅ **APIs**: On your machine (dotnet run or Visual Studio)
- ✅ **Frontend**: On your machine (npm run dev)

### 🔧 Initial Setup

1. **Start databases:**

```bash
cd C:\Users\devke\source\bla-task-management-system
docker-compose up -d mongodb postgres
```

2. **Verify they're healthy:**

```bash
docker ps
# You should see: mongodb (healthy), postgres (healthy)
```

### 💻 Option A: With dotnet run

**Start Tasks API:**

```powershell
cd apps/tasks-api/src/TasksApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

**Start Users API (when implemented):**

```powershell
cd apps/users-api/src/UsersApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

### 💻 Option B: With Visual Studio / Rider

1. Open `BlaTaskManagement.sln`
2. Configure startup projects:
   - Right-click on solution → **Set Startup Projects**
   - Select **Multiple startup projects**
   - Mark: `TasksApi.WebApi` and `UsersApi.WebApi` as **Start**
3. Press **F5** or click **Run**

### 🔄 Workflow (Development)

```
1. Make changes in the code
   ↓
2. Save the file (Ctrl+S)
   ↓
3. Hot Reload detects changes (~2-5 seconds)
   ↓
4. API recompiles automatically
   ↓
5. Test in Postman/Swagger
   ↓
6. Repeat the cycle
```

**Practical example:**

```csharp
// 1. Edit TasksController.cs
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
{
    // Your code here
}

// 2. Save
// 3. Wait 2-5 seconds
// 4. Test in Postman: POST /api/tasks
// 5. If it doesn't work, debug with breakpoints
```

### 🧪 Run Tests

```bash
# All tests
dotnet test

# Tests from a specific project
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Watch mode (re-runs on each change)
dotnet watch test
```

### 🛑 Stop Services

```powershell
# Stop API (Ctrl+C in the console)
# Or close Visual Studio

# Stop databases
docker-compose stop mongodb postgres
```

---

## 🐳 Workflow 2: Everything in Docker

### ✅ Advantages

- 🎯 **Identical to production environment**
- 📦 **Everything isolated and consistent**
- 🤝 **Easy to share with the team**
- 🚀 **One command to start everything**
- 🔗 **Automatic networking between services**

### ❌ Disadvantages

- ⏱️ **Rebuild on every change**: ~30-60 seconds
- 🐛 **Complex debugging**: Requires remote debugging
- 🔄 **No Hot Reload** by default
- 📝 **Less clear logs**: Mixed in docker-compose

### 🔧 Initial Setup

**Build images:**

```bash
cd C:\Users\devke\source\bla-task-management-system

# Build all services
docker-compose build

# Or individual build
docker-compose build tasks-api
docker-compose build users-api
```

### 🚀 Start All Services

```bash
# Start everything
docker-compose up -d

# See logs
docker-compose logs -f

# See logs of a specific service
docker-compose logs -f tasks-api
```

**Verify:**

```bash
docker-compose ps

# You should see:
# - tasks-mongodb (healthy)
# - users-postgres (healthy)
# - tasks-api (running)
# - users-api (running)
# - web (running - when implemented)
```

### 🔄 Workflow (With Docker)

```
1. Make changes in the code
   ↓
2. Save the file
   ↓
3. Rebuild Docker image (~30-60 seconds)
   docker-compose build tasks-api
   ↓
4. Restart container (~5-10 seconds)
   docker-compose up -d tasks-api
   ↓
5. Test in Postman/Swagger
   ↓
6. Repeat the cycle
```

**Practical example:**

```bash
# 1. Edit TasksController.cs
code apps/tasks-api/src/TasksApi.WebApi/Controllers/TasksController.cs

# 2. Save

# 3. Rebuild
docker-compose build tasks-api

# 4. Restart
docker-compose up -d tasks-api

# 5. View logs to verify it started
docker-compose logs -f tasks-api

# 6. Test in Postman
```

### 🔄 Quick Rebuild vs Full Rebuild

**Quick rebuild (cache):**

```bash
docker-compose build tasks-api
docker-compose up -d tasks-api
# Time: ~10-20 seconds (uses cache)
```

**Full rebuild (no cache):**

```bash
docker-compose build --no-cache tasks-api
docker-compose up -d tasks-api
# Time: ~60-90 seconds (downloads everything again)
```

### 🛑 Stop Services

```bash
# Stop all
docker-compose down

# Stop but keep volumes (data)
docker-compose down

# Stop and remove volumes (CAUTION: deletes data)
docker-compose down -v

# Stop only APIs (keep databases)
docker-compose stop tasks-api users-api
```

---

## 📊 Comparison

| Aspect | Local Development | Everything in Docker |
|---------|-----------------|---------------|
| **Hot Reload** | ✅ Yes | ❌ No |
| **Debugging** | ✅ Easy (breakpoints) | ⚠️ Complex (remote) |
| **Time per change** | ⚡ 2-5 seconds | ⏱️ 30-60 seconds |
| **TDD** | ✅ Instant | ⚠️ Slow |
| **Production similarity** | ⚠️ Medium | ✅ High |
| **Initial setup** | ⚡ Fast | ⏱️ Slow (build) |
| **Logs** | ✅ Clear | ⚠️ Mixed |
| **Networking** | ⚠️ Manual (localhost) | ✅ Automatic |
| **Isolation** | ⚠️ Uses your machine | ✅ Containers |
| **Portability** | ⚠️ Medium | ✅ High |

---

## 🎯 When to Use Each One

### 👨‍💻 Use Local Development when:

- ✅ You're implementing a feature (US-02, US-03, etc.)
- ✅ You're doing TDD (red-green-refactor cycle)
- ✅ You need frequent debugging
- ✅ You're rapidly iterating on a solution
- ✅ You're constantly running unit tests
- ✅ You're actively developing (90% of the time)

**Example:**
```
"I'm going to implement US-02: Create Task"
→ Use local development
→ TDD: test → code → test → refactor
→ Hot reload for quick feedback
```

### 🐳 Use Docker Compose when:

- ✅ You're going to demo the complete system
- ✅ You need integration testing between services
- ✅ You're going to deploy/PR (final verification)
- ✅ You're verifying everything works together
- ✅ Someone else is going to test your work
- ✅ Before marking a feature as "done"

**Example:**
```
"I finished implementing US-02, US-03, US-05, US-06"
→ Use Docker Compose
→ Verify everything works together
→ Demo to client/team
→ Commit and PR
```

---

## 🔧 Useful Commands

### Local Development

```powershell
# Start only databases
docker-compose up -d mongodb postgres

# Verify status
docker ps
Get-Process -Name dotnet

# Start API manually
cd apps/tasks-api/src/TasksApi.WebApi
dotnet run

# Watch mode (auto-recompiles)
dotnet watch run

# Tests in watch mode
dotnet watch test

# View MongoDB logs
docker logs -f tasks-mongodb

# Stop API (Ctrl+C)
# Stop DBs
docker-compose stop mongodb postgres
```

### Docker Compose

```bash
# Build everything
docker-compose build

# Start everything
docker-compose up -d

# View logs
docker-compose logs -f
docker-compose logs -f tasks-api

# Rebuild and restart an API
docker-compose build tasks-api && docker-compose up -d tasks-api

# View status
docker-compose ps

# Enter a container
docker exec -it tasks-api bash

# Stop everything
docker-compose down

# Stop and clean everything
docker-compose down -v --remove-orphans
```

### Combined (Recommended)

```powershell
# DBs in Docker, APIs local
docker-compose up -d mongodb postgres
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# In another terminal
cd apps/users-api/src/UsersApi.WebApi
dotnet watch run
```

---

## 💡 Final Recommendation

**For this project (Technical Interview):**

### During Development (Now):

```
✅ Databases: Docker Compose
✅ APIs: Local Development (dotnet run or Visual Studio)
✅ Tests: Local (dotnet test)
```

**Why:**
- Fast iteration for TDD
- Immediate feedback
- Easy debugging

### Before Demo/Delivery:

```
✅ Everything: Docker Compose
✅ docker-compose up -d
✅ Complete verification
```

**Why:**
- Simulates production
- Verifies integrations
- Easy to demonstrate

---

## 🚀 Practical Example: Implement US-02

### Phase 1: Development (Local)

```bash
# 1. DBs in Docker
docker-compose up -d mongodb postgres

# 2. Local API with watch
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# 3. In another terminal: tests in watch
cd apps/tasks-api/tests/TasksApi.Application.Tests
dotnet watch test

# 4. Iterate:
#    - Write test
#    - Test fails (red)
#    - Write code
#    - Test passes (green)
#    - Refactor
#    - Repeat
```

### Phase 2: Verification (Docker)

```bash
# 1. Stop local API (Ctrl+C)

# 2. Build and start everything
docker-compose build tasks-api
docker-compose up -d

# 3. Verify
docker-compose logs -f tasks-api
curl http://localhost:5001/api/tasks

# 4. If it works, commit and push
git add .
git commit -m "feat: Implement US-02 Create Task"
git push
```

---

**Last updated:** 2026-06-09  
**Document:** Development Workflows
