# 🔄 Workflows de Desarrollo - BLA Task Management System

Este documento explica los dos workflows principales de desarrollo y cuándo usar cada uno.

---

## 📋 Tabla de Contenidos

1. [Setup Actual](#setup-actual)
2. [Workflow 1: Desarrollo Local (Recomendado)](#workflow-1-desarrollo-local-recomendado)
3. [Workflow 2: Todo en Docker](#workflow-2-todo-en-docker)
4. [Comparación](#comparación)
5. [Cuándo Usar Cada Uno](#cuándo-usar-cada-uno)
6. [Comandos Útiles](#comandos-útiles)

---

## 🎯 Setup Actual

**Estado actual del proyecto:**

```
┌─────────────────────────────────────────────────────┐
│  🐳 EN DOCKER:                                      │
│     • MongoDB (port 27017)                          │
│     • PostgreSQL (port 5432)                        │
│                                                      │
│  🖥️  EN TU MÁQUINA (local):                        │
│     • Tasks API (dotnet run - port 5077)           │
│     • Proceso en segundo plano (detached)           │
│                                                      │
│  🔲 NO INICIADAS:                                   │
│     • Users API                                      │
│     • Frontend (Vue.js)                             │
└─────────────────────────────────────────────────────┘
```

**Verificar estado:**

```powershell
# Ver procesos .NET
Get-Process -Name dotnet

# Ver contenedores Docker
docker ps

# Probar API
curl http://localhost:5077/api/tasks?userId=00000000-0000-0000-0000-000000000001
```

---

## 🏗️ Workflow 1: Desarrollo Local (Recomendado)

### ✅ Ventajas

- ⚡ **Hot Reload**: Cambios se aplican automáticamente sin reiniciar
- 🐛 **Debugging completo**: Breakpoints, inspección de variables
- 🚀 **Rápido**: Sin rebuild de imágenes Docker
- 🧪 **TDD-friendly**: Tests se ejecutan instantáneamente
- 📝 **Logs claros**: Salida directa en consola

### 📝 Cómo Funciona

**Servicios:**
- ✅ **Bases de datos**: En Docker (MongoDB + PostgreSQL)
- ✅ **APIs**: En tu máquina (dotnet run o Visual Studio)
- ✅ **Frontend**: En tu máquina (npm run dev)

### 🔧 Setup Inicial

1. **Iniciar bases de datos:**

```bash
cd C:\Users\devke\source\bla-task-management-system
docker-compose up -d mongodb postgres
```

2. **Verificar que estén healthy:**

```bash
docker ps
# Deberías ver: mongodb (healthy), postgres (healthy)
```

### 💻 Opción A: Con dotnet run

**Iniciar Tasks API:**

```powershell
cd apps/tasks-api/src/TasksApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

**Iniciar Users API (cuando esté implementada):**

```powershell
cd apps/users-api/src/UsersApi.WebApi
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

### 💻 Opción B: Con Visual Studio / Rider

1. Abre `BlaTaskManagement.sln`
2. Configura proyectos de inicio:
   - Click derecho en solution → **Set Startup Projects**
   - Selecciona **Multiple startup projects**
   - Marca: `TasksApi.WebApi` y `UsersApi.WebApi` como **Start**
3. Presiona **F5** o click en **Run**

### 🔄 Flujo de Trabajo (Desarrollo)

```
1. Haces cambios en el código
   ↓
2. Guardas el archivo (Ctrl+S)
   ↓
3. Hot Reload detecta cambios (~2-5 segundos)
   ↓
4. API se recompila automáticamente
   ↓
5. Pruebas en Postman/Swagger
   ↓
6. Repites el ciclo
```

**Ejemplo práctico:**

```csharp
// 1. Editas TasksController.cs
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
{
    // Tu código aquí
}

// 2. Guardas
// 3. Esperas 2-5 segundos
// 4. Pruebas en Postman: POST /api/tasks
// 5. Si no funciona, debuggeas con breakpoints
```

### 🧪 Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Tests de un proyecto específico
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Watch mode (re-ejecuta en cada cambio)
dotnet watch test
```

### 🛑 Detener Servicios

```powershell
# Detener API (Ctrl+C en la consola)
# O cerrar Visual Studio

# Detener bases de datos
docker-compose stop mongodb postgres
```

---

## 🐳 Workflow 2: Todo en Docker

### ✅ Ventajas

- 🎯 **Entorno idéntico a producción**
- 📦 **Todo aislado y consistente**
- 🤝 **Fácil compartir con el equipo**
- 🚀 **Un comando para levantar todo**
- 🔗 **Networking automático entre servicios**

### ❌ Desventajas

- ⏱️ **Rebuild en cada cambio**: ~30-60 segundos
- 🐛 **Debugging complejo**: Requiere remote debugging
- 🔄 **Sin Hot Reload** por defecto
- 📝 **Logs menos claros**: Mezclados en docker-compose

### 🔧 Setup Inicial

**Construir imágenes:**

```bash
cd C:\Users\devke\source\bla-task-management-system

# Build de todos los servicios
docker-compose build

# O build individual
docker-compose build tasks-api
docker-compose build users-api
```

### 🚀 Iniciar Todos los Servicios

```bash
# Levantar todo
docker-compose up -d

# Ver logs
docker-compose logs -f

# Ver logs de un servicio específico
docker-compose logs -f tasks-api
```

**Verificar:**

```bash
docker-compose ps

# Deberías ver:
# - tasks-mongodb (healthy)
# - users-postgres (healthy)
# - tasks-api (running)
# - users-api (running)
# - web (running - cuando se implemente)
```

### 🔄 Flujo de Trabajo (Con Docker)

```
1. Haces cambios en el código
   ↓
2. Guardas el archivo
   ↓
3. Rebuild de la imagen Docker (~30-60 segundos)
   docker-compose build tasks-api
   ↓
4. Restart del contenedor (~5-10 segundos)
   docker-compose up -d tasks-api
   ↓
5. Pruebas en Postman/Swagger
   ↓
6. Repites el ciclo
```

**Ejemplo práctico:**

```bash
# 1. Editas TasksController.cs
code apps/tasks-api/src/TasksApi.WebApi/Controllers/TasksController.cs

# 2. Guardas

# 3. Rebuild
docker-compose build tasks-api

# 4. Restart
docker-compose up -d tasks-api

# 5. Ver logs para verificar que inició
docker-compose logs -f tasks-api

# 6. Pruebas en Postman
```

### 🔄 Rebuild Rápido vs Completo

**Rebuild rápido (cache):**

```bash
docker-compose build tasks-api
docker-compose up -d tasks-api
# Tiempo: ~10-20 segundos (usa cache)
```

**Rebuild completo (sin cache):**

```bash
docker-compose build --no-cache tasks-api
docker-compose up -d tasks-api
# Tiempo: ~60-90 segundos (descarga todo de nuevo)
```

### 🛑 Detener Servicios

```bash
# Detener todos
docker-compose down

# Detener pero mantener volúmenes (datos)
docker-compose down

# Detener y eliminar volúmenes (CUIDADO: borra datos)
docker-compose down -v

# Detener solo APIs (mantener bases de datos)
docker-compose stop tasks-api users-api
```

---

## 📊 Comparación

| Aspecto | Desarrollo Local | Todo en Docker |
|---------|-----------------|---------------|
| **Hot Reload** | ✅ Sí | ❌ No |
| **Debugging** | ✅ Fácil (breakpoints) | ⚠️ Complejo (remote) |
| **Tiempo por cambio** | ⚡ 2-5 segundos | ⏱️ 30-60 segundos |
| **TDD** | ✅ Instantáneo | ⚠️ Lento |
| **Similitud con prod** | ⚠️ Media | ✅ Alta |
| **Setup inicial** | ⚡ Rápido | ⏱️ Lento (build) |
| **Logs** | ✅ Claros | ⚠️ Mezclados |
| **Networking** | ⚠️ Manual (localhost) | ✅ Automático |
| **Aislamiento** | ⚠️ Usa tu máquina | ✅ Contenedores |
| **Portabilidad** | ⚠️ Media | ✅ Alta |

---

## 🎯 Cuándo Usar Cada Uno

### 👨‍💻 Usa Desarrollo Local cuando:

- ✅ Estás implementando una feature (US-02, US-03, etc.)
- ✅ Estás haciendo TDD (ciclo rojo-verde-refactor)
- ✅ Necesitas debugging frecuente
- ✅ Estás iterando rápidamente en una solución
- ✅ Estás ejecutando tests unitarios constantemente
- ✅ Estás desarrollando activamente (90% del tiempo)

**Ejemplo:**
```
"Voy a implementar US-02: Create Task"
→ Usa desarrollo local
→ TDD: test → código → test → refactor
→ Hot reload para feedback rápido
```

### 🐳 Usa Docker Compose cuando:

- ✅ Vas a hacer un demo del sistema completo
- ✅ Necesitas testing de integración entre servicios
- ✅ Vas a hacer deploy/PR (verificación final)
- ✅ Estás verificando que todo funcione junto
- ✅ Alguien más va a probar tu trabajo
- ✅ Antes de marcar una feature como "done"

**Ejemplo:**
```
"Terminé de implementar US-02, US-03, US-05, US-06"
→ Usa Docker Compose
→ Verifica que todo funcione junto
→ Demo al cliente/equipo
→ Haz commit y PR
```

---

## 🔧 Comandos Útiles

### Desarrollo Local

```powershell
# Iniciar solo bases de datos
docker-compose up -d mongodb postgres

# Verificar estado
docker ps
Get-Process -Name dotnet

# Iniciar API manualmente
cd apps/tasks-api/src/TasksApi.WebApi
dotnet run

# Watch mode (auto-recompila)
dotnet watch run

# Tests en watch mode
dotnet watch test

# Ver logs de MongoDB
docker logs -f tasks-mongodb

# Detener API (Ctrl+C)
# Detener DBs
docker-compose stop mongodb postgres
```

### Docker Compose

```bash
# Build todo
docker-compose build

# Levantar todo
docker-compose up -d

# Ver logs
docker-compose logs -f
docker-compose logs -f tasks-api

# Rebuild y restart de una API
docker-compose build tasks-api && docker-compose up -d tasks-api

# Ver estado
docker-compose ps

# Entrar a un contenedor
docker exec -it tasks-api bash

# Detener todo
docker-compose down

# Detener y limpiar todo
docker-compose down -v --remove-orphans
```

### Combinado (Recomendado)

```powershell
# DBs en Docker, APIs local
docker-compose up -d mongodb postgres
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# En otra terminal
cd apps/users-api/src/UsersApi.WebApi
dotnet watch run
```

---

## 💡 Recomendación Final

**Para este proyecto (Technical Interview):**

### Durante Desarrollo (Ahora):

```
✅ Bases de datos: Docker Compose
✅ APIs: Desarrollo Local (dotnet run o Visual Studio)
✅ Tests: Local (dotnet test)
```

**Por qué:**
- Iteración rápida para TDD
- Feedback inmediato
- Debugging fácil

### Antes del Demo/Entrega:

```
✅ Todo: Docker Compose
✅ docker-compose up -d
✅ Verificación completa
```

**Por qué:**
- Simula producción
- Verifica integraciones
- Fácil de demostrar

---

## 🚀 Ejemplo Práctico: Implementar US-02

### Fase 1: Desarrollo (Local)

```bash
# 1. DBs en Docker
docker-compose up -d mongodb postgres

# 2. API local con watch
cd apps/tasks-api/src/TasksApi.WebApi
dotnet watch run

# 3. En otra terminal: tests en watch
cd apps/tasks-api/tests/TasksApi.Application.Tests
dotnet watch test

# 4. Iterar:
#    - Escribes test
#    - Test falla (rojo)
#    - Escribes código
#    - Test pasa (verde)
#    - Refactorizas
#    - Repites
```

### Fase 2: Verificación (Docker)

```bash
# 1. Detener API local (Ctrl+C)

# 2. Build y levantar todo
docker-compose build tasks-api
docker-compose up -d

# 3. Verificar
docker-compose logs -f tasks-api
curl http://localhost:5001/api/tasks

# 4. Si funciona, commit y push
git add .
git commit -m "feat: Implement US-02 Create Task"
git push
```

---

**Última actualización:** 2026-06-09  
**Documento:** Workflows de desarrollo
