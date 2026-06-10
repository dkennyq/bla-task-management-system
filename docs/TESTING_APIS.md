# 🧪 Testing APIs - BLA Task Management System

Este documento contiene instrucciones para ejecutar y probar las APIs del sistema.

## 📋 Tabla de Contenidos

1. [Prerequisitos](#prerequisitos)
2. [Ejecutar Docker (Bases de Datos)](#ejecutar-docker-bases-de-datos)
3. [Ejecutar APIs Localmente](#ejecutar-apis-localmente)
4. [Probar con Postman](#probar-con-postman)
5. [Probar con Swagger](#probar-con-swagger)
6. [Probar con curl](#probar-con-curl)

---

## ✅ Prerequisitos

- [x] Docker Desktop instalado y corriendo
- [x] .NET 8 SDK instalado
- [x] Postman instalado (opcional)
- [x] Git Bash o PowerShell

---

## 🐳 Ejecutar Docker (Bases de Datos)

### 1️⃣ Iniciar contenedores de bases de datos

```bash
cd C:\Users\devke\source\bla-task-management-system
docker-compose up -d mongodb postgres
```

### 2️⃣ Verificar que los contenedores estén corriendo

```bash
docker ps
```

Deberías ver:
- `tasks-mongodb` (puerto 27017) - STATUS: Up (healthy)
- `users-postgres` (puerto 5432) - STATUS: Up (healthy)

### 3️⃣ Ver logs de los contenedores (opcional)

```bash
# MongoDB logs
docker logs tasks-mongodb

# PostgreSQL logs
docker logs users-postgres
```

---

## 🚀 Ejecutar APIs Localmente

### ✅ Tasks API (Puerto 5077)

#### Opción 1: PowerShell

```powershell
cd C:\Users\devke\source\bla-task-management-system\apps\tasks-api\src\TasksApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://localhost:5001"
dotnet run
```

**Nota:** La API se ejecutará en el puerto **5077** (definido en `launchSettings.json`), no en 5001.

#### Opción 2: Visual Studio / Rider

1. Abre la solución `BlaTaskManagement.sln`
2. Configura `TasksApi.WebApi` como proyecto de inicio
3. Presiona F5 o haz clic en "Run"

#### Verificar que está corriendo

```bash
curl http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001
```

Respuesta esperada: `[]` (array vacío - no hay tareas)

---

### 🔲 Users API (Puerto 5078) - NO IMPLEMENTADA AÚN

Esta API aún no está implementada. Los endpoints están definidos en Issues #5-8.

---

## 📬 Probar con Postman

### 1️⃣ Importar la colección

1. Abre Postman
2. Click en **Import** (esquina superior izquierda)
3. Selecciona el archivo: `docs/POSTMAN_COLLECTION.json`
4. La colección se importará con 9 requests pre-configurados

### 2️⃣ Variables de entorno (ya configuradas)

La colección incluye estas variables:
- `userId`: `00000000-0000-0000-0000-000000000001`
- `taskId`: (vacío - se actualizará después de crear una tarea)
- `jwt_token`: (vacío - se actualizará después de login)

### 3️⃣ Probar endpoint GET /api/tasks

1. Expande la carpeta **"Tasks API"**
2. Selecciona **"Get All Tasks"**
3. Click en **Send**
4. Deberías ver respuesta: `[]` (Status: 200 OK)

### 4️⃣ Probar endpoints pendientes

Los siguientes endpoints aún **NO ESTÁN IMPLEMENTADOS**:
- ❌ POST /api/tasks (Create Task) - Issue #1
- ❌ PUT /api/tasks/{id} (Update Task) - Issue #2
- ❌ DELETE /api/tasks/{id} (Delete Task) - Issue #3
- ❌ GET /api/tasks/{id} (Get Task By Id) - Issue #4

Estos responderán con **404 Not Found** hasta que se implementen.

---

## 📖 Probar con Swagger

### 1️⃣ Abrir Swagger UI

Abre tu navegador y ve a:

```
http://localhost:5077/swagger
```

### 2️⃣ Explorar endpoints disponibles

Verás la documentación interactiva de la API con:
- **GET /api/tasks** - ✅ Implementado
- Otros endpoints aparecerán aquí cuando se implementen

### 3️⃣ Probar un endpoint

1. Click en **GET /api/tasks**
2. Click en **Try it out**
3. Ingresa un `userId` (ej: `00000000-0000-0000-0000-000000000001`)
4. Click en **Execute**
5. Verás la respuesta en la sección **Response body**

---

## 🔧 Probar con curl

### GET /api/tasks (Todas las tareas de un usuario)

```bash
# PowerShell
curl.exe http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001

# Git Bash / Linux / Mac
curl "http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001"
```

**Respuesta esperada:**
```json
[]
```

### GET /api/tasks (Con formato JSON)

```powershell
# PowerShell
curl.exe "http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001" -s | ConvertFrom-Json | ConvertTo-Json -Depth 10
```

### POST /api/tasks (Crear tarea) - NO IMPLEMENTADO

```bash
curl -X POST http://localhost:5077/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Task",
    "description": "Test Description",
    "status": "Pending",
    "dueDate": "2026-06-15T23:59:59Z",
    "userId": "00000000-0000-0000-0000-000000000001"
  }'
```

**Respuesta actual:** `404 Not Found` (Issue #1 pendiente)

---

## 🔍 Troubleshooting

### ❌ Error: "No such host is known" (MongoDB)

**Causa:** La API está intentando conectarse a `mongodb://mongodb:27017` (hostname de Docker) en lugar de `localhost`.

**Solución:** Verifica que `appsettings.Development.json` tenga:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "tasksdb"
  }
}
```

### ❌ Error: "Connection refused" (Puerto incorrecto)

**Causa:** Las APIs usan los puertos definidos en `launchSettings.json`, no los de las variables de entorno.

**Solución:**
- Tasks API corre en puerto **5077** (no 5001)
- Users API correrá en puerto **5078** (no 5002)

### ❌ Docker containers no inician

```bash
# Ver logs de errores
docker-compose logs mongodb
docker-compose logs postgres

# Reiniciar contenedores
docker-compose down
docker-compose up -d mongodb postgres
```

### ❌ API no compila

```bash
# Limpiar y reconstruir
cd C:\Users\devke\source\bla-task-management-system
dotnet clean BlaTaskManagement.sln
dotnet build BlaTaskManagement.sln
```

---

## 📊 Estado Actual de Implementación

### ✅ Tasks API

| Endpoint | Método | Estado | Issue |
|----------|--------|--------|-------|
| `/api/tasks?userId={guid}` | GET | ✅ Implementado | - |
| `/api/tasks/{id}` | GET | 🔲 Pendiente | #4 |
| `/api/tasks` | POST | 🔲 Pendiente | #1 |
| `/api/tasks/{id}` | PUT | 🔲 Pendiente | #2 |
| `/api/tasks/{id}` | DELETE | 🔲 Pendiente | #3 |

### 🔲 Users API (No iniciada)

| Endpoint | Método | Estado | Issue |
|----------|--------|--------|-------|
| `/api/users/register` | POST | 🔲 Pendiente | #5 |
| `/api/users/login` | POST | 🔲 Pendiente | #6 |
| `/api/users/me` | GET | 🔲 Pendiente | #7 |
| `/api/users` | GET | 🔲 Pendiente | #8 |

---

## 🎯 Siguiente Paso

1. ✅ Docker corriendo (MongoDB + PostgreSQL)
2. ✅ Tasks API corriendo y respondiendo
3. ✅ Postman collection lista
4. 🔲 **Implementar US-02: Create Task Endpoint (Issue #1)**

```bash
gh issue view 1
gh issue edit 1 --add-assignee @me
git checkout -b feature/us-02-create-task
```

Ver `docs/USER_STORIES.md` para guía de implementación paso a paso.

---

## 📚 Recursos Adicionales

- [USER_STORIES.md](./USER_STORIES.md) - Historias de usuario con guías de implementación
- [GITHUB_TASKS_SETUP.md](./GITHUB_TASKS_SETUP.md) - Configuración de GitHub Issues
- [GITHUB_PROJECT_SETUP.md](./GITHUB_PROJECT_SETUP.md) - Configuración de GitHub Project
- [Swagger UI](http://localhost:5077/swagger) - Documentación interactiva de la API

---

**Última actualización:** 2026-06-09  
**Autor:** BLA Task Management Team  
**Proyecto:** Technical Interview Exercise
