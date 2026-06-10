# 🧪 Testing APIs - BLA Task Management System

This document contains instructions to run and test the system's APIs.

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Run Docker (Databases)](#run-docker-databases)
3. [Run APIs Locally](#run-apis-locally)
4. [Authentication - JWT Tokens](#authentication---jwt-tokens)
5. [Test with Postman](#test-with-postman)
6. [Test with Swagger](#test-with-swagger)
7. [Test with curl](#test-with-curl)

---

## ✅ Prerequisites

- [ ] Docker Desktop installed and running
- [ ] .NET 8 SDK installed
- [ ] Postman installed (optional)
- [ ] Git Bash or PowerShell

---

## 🐳 Run Docker (Databases)

### 1️⃣ Start database containers

```bash
cd C:\Users\devke\source\bla-task-management-system
docker-compose up -d mongodb postgres
```

### 2️⃣ Verify containers are running

```bash
docker ps
```

You should see:
- `tasks-mongodb` (port 27017) - STATUS: Up (healthy)
- `users-postgres` (port 5432) - STATUS: Up (healthy)

### 3️⃣ View container logs (optional)

```bash
# MongoDB logs
docker logs tasks-mongodb

# PostgreSQL logs
docker logs users-postgres
```

---

## 🚀 Run APIs Locally

### ✅ Tasks API (Port 5077)

#### Option 1: PowerShell

```powershell
cd C:\Users\devke\source\bla-task-management-system\apps\tasks-api\src\TasksApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://localhost:5001"
dotnet run
```

#### Option 2: Visual Studio / Rider

1. Open solution `BlaTaskManagement.sln`
2. Set `TasksApi.WebApi` as startup project
3. Press F5 or click "Run"

#### Verify it's running

```bash
curl http://localhost:5077/api/tasks
```

**Note:** Tasks API now requires JWT authentication - you'll get `401 Unauthorized` without a token.
See [Authentication section](#authentication---jwt-tokens) below.

---

### ✅ Users API (Port 5078)

#### Run locally

```powershell
cd C:\Users\devke\source\bla-task-management-system\apps\users-api\src\UsersApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://localhost:5002"
dotnet run
```

#### Verify it's running

```bash
curl http://localhost:5078/api/users/login -X POST -H "Content-Type: application/json" -d "{\"email\":\"admin@taskmanagement.com\",\"password\":\"Password123!\"}"
```

Expected response:
```json
{
  "userId": "00000000-0000-0000-0000-000000000001",
  "email": "admin@taskmanagement.com",
  "fullName": "Admin User",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-06-11T...Z"
}
```

---

## 🔐 Authentication - JWT Tokens

All API endpoints (except login and register) require JWT authentication.

### Authentication Flow

```
1. POST /api/users/login → Get JWT token
2. Include token in Authorization header for all subsequent requests
3. Backend validates token and extracts userId from claims
4. 401 Unauthorized for missing/invalid/expired tokens
```

### Get a Token

```bash
# Login with demo credentials
curl -X POST http://localhost:5078/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@taskmanagement.com","password":"Password123!"}'

# Save the token from the response
```

### Use Token in Requests

```bash
curl http://localhost:5077/api/tasks \
  -H "Authorization: Bearer <your-jwt-token>"
```

---

## 📬 Test with Postman

### 1️⃣ Import collection

1. Open Postman
2. Click **Import** (top left corner)
3. Select file: `docs/POSTMAN_COLLECTION.json`
4. Collection will be imported with pre-configured requests

### 2️⃣ Environment variables (pre-configured)

The collection includes these variables:
- `userId`: `00000000-0000-0000-0000-000000000001`
- `taskId`: (empty - updates after task creation)
- `jwt_token`: (empty - updates after login)

### 3️⃣ First: Authenticate

1. Expand the **"Users API"** folder
2. Select **"Login"**
3. Click **Send**
4. Copy `token` from response
5. Set collection variable `jwt_token` with this value

### 4️⃣ Then: Test protected endpoints

All other requests automatically include the `jwt_token` in the Authorization header.

---

## 📖 Test with Swagger

### Tasks API Swagger

```
http://localhost:5077/swagger
```

1. Open Swagger UI
2. Click **Authorize** button (top right)
3. Enter your JWT token: `Bearer <your-token>`
4. Click **Authorize**
5. Now you can test any endpoint

### Users API Swagger

```
http://localhost:5078/swagger
```

Same authorization flow as Tasks API.

---

## 🔧 Test with curl

### Step 1: Authenticate and get token

```bash
# PowerShell
$response = curl.exe -X POST http://localhost:5078/api/users/login `
  -H "Content-Type: application/json" `
  -d '{"email":"admin@taskmanagement.com","password":"Password123!"}'

$token = ($response | ConvertFrom-Json).token
```

### Step 2: Use token in subsequent requests

```bash
# GET all tasks (authenticated)
curl.exe http://localhost:5077/api/tasks -H "Authorization: Bearer $token"
```

### Step 3: Create a task (authenticated)

```bash
curl.exe -X POST http://localhost:5077/api/tasks `
  -H "Content-Type: application/json" `
  -H "Authorization: Bearer $token" `
  -d '{"title":"Test Task","description":"Test Description","priority":"Medium","dueDate":"2026-06-15T23:59:59Z"}'
```

Note: `userId` is no longer required in the request body - it's extracted from the JWT token.

### Step 4: Verify authentication works

```bash
# Without token (should fail with 401)
curl.exe http://localhost:5077/api/tasks
# Expected: 401 Unauthorized
```

---

## 📊 Current Implementation Status

### ✅ Tasks API (Protected with JWT)

| Endpoint | Method | Status | Issue |
|----------|--------|--------|-------|
| `/api/tasks` | GET | ✅ Implemented (Auth required) | #19 |
| `/api/tasks/{id}` | GET | ✅ Implemented (Auth required) | #4 |
| `/api/tasks` | POST | ✅ Implemented (Auth required) | #1 |
| `/api/tasks/{id}` | PUT | ✅ Implemented (Auth required) | #2 |
| `/api/tasks/{id}` | DELETE | ✅ Implemented (Auth required) | #3 |

### ✅ Users API (With JWT Authentication)

| Endpoint | Method | Status | Issue |
|----------|--------|--------|-------|
| `/api/users/register` | POST | ✅ Implemented (Public) | #5 |
| `/api/users/login` | POST | ✅ Implemented (Public) | #6 |
| `/api/users/me` | GET | ✅ Implemented (Auth required) | #7 |

### Demo Credentials

| Email | Password | User ID |
|-------|----------|---------|
| admin@taskmanagement.com | Password123! | 00000000-0000-0000-0000-000000000001 |
| john.doe@example.com | Password123! | 00000000-0000-0000-0000-000000000002 |
| jane.smith@example.com | Password123! | 00000000-0000-0000-0000-000000000003 |

---

## 🎯 Next Steps

1. ✅ Docker running (MongoDB + PostgreSQL)
2. ✅ Tasks API running and responding
3. ✅ Users API with authentication
4. ✅ JWT Authentication implemented
5. 🔲 **Start frontend development (Issues #9, #10)**

---

## 📚 Additional Resources

- [API_SECURITY_GUIDE.md](./API_SECURITY_GUIDE.md) - JWT implementation guide
- [USER_STORIES.md](./USER_STORIES.md) - User stories with implementation guides
- [Swagger UI - Tasks API](http://localhost:5077/swagger)
- [Swagger UI - Users API](http://localhost:5078/swagger)

---

**Last updated:** 2026-06-10  
**Author:** BLA Task Management Team  
**Project:** Technical Interview Exercise
