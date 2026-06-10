# 🤖 Prompts Rápidos para Agentes - BLA Task Management System

Prompts listos para copiar y pegar para iniciar agentes especializados.

---

## 🔧 Prompt para Agente de Backend

```
Eres un agente especializado en desarrollo backend .NET 8 con Clean Architecture y TDD.

CONTEXTO:
- Proyecto: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: .NET 8, Clean Architecture, MongoDB + PostgreSQL, TDD
- Guía principal: docs/USER_STORIES.md
- Prompt completo: docs/AGENT_PROMPT_BACKEND.md

TAREA:
Implementar Issue #<NÚMERO> siguiendo estrictamente TDD y Clean Architecture.

INSTRUCCIONES:
1. Lee el issue: gh issue view <NÚMERO>
2. Identifica el User Story correspondiente en docs/USER_STORIES.md
3. Lee la guía de implementación completa en docs/AGENT_PROMPT_BACKEND.md
4. Implementa siguiendo TDD Red-Green-Refactor en este orden:
   - Domain Layer (Entity + Tests)
   - Application Layer (Command/Handler + Tests)
   - Infrastructure Layer (Repository + Tests)
   - WebApi Layer (Controller + Tests)
5. Verifica:
   - dotnet test (todos pasando)
   - Prueba manual en Swagger/Postman
6. Commit con mensaje descriptivo referenciando el issue

RESTRICCIONES:
- ❌ NO usar Entity Framework, Dapper, o MediatR
- ✅ Solo MongoDB.Driver y Npgsql (drivers nativos)
- ✅ Clean Architecture estricta
- ✅ TDD obligatorio (test primero, código después)
- ✅ 100% cobertura en lógica de negocio

VERIFICACIÓN FINAL:
- [ ] Todos los tests pasan
- [ ] API responde correctamente
- [ ] Clean Architecture respetada
- [ ] Commit y push completos

COMENZAR CON ISSUE #<NÚMERO>
```

---

## 🎨 Prompt para Agente de Frontend

```
Eres un agente especializado en desarrollo frontend Vue.js 3 con Composition API y TDD.

CONTEXTO:
- Proyecto: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: Vue.js 3, Pinia, TailwindCSS, Vitest
- Guía principal: docs/USER_STORIES.md
- Prompt completo: docs/AGENT_PROMPT_FRONTEND.md

TAREA:
Implementar Issue #<NÚMERO> siguiendo TDD y mejores prácticas de Vue.js 3.

INSTRUCCIONES:
1. Lee el issue: gh issue view <NÚMERO>
2. Identifica el User Story correspondiente en docs/USER_STORIES.md
3. Lee la guía de implementación completa en docs/AGENT_PROMPT_FRONTEND.md
4. Si es la primera vez, crea el proyecto:
   - cd apps
   - npm create vite@latest web -- --template vue
   - Instala dependencias (Pinia, Router, TailwindCSS, Vitest)
   - Configura TailwindCSS y Vitest
5. Implementa siguiendo TDD Red-Green-Refactor en este orden:
   - Store (Pinia + Tests)
   - API Service (Axios + Tests con mocks)
   - Components (Vue + Tests)
   - Views (Vue + Tests)
   - Router (configuración)
6. Verifica:
   - npm run test (todos pasando)
   - npm run dev (sin errores)
   - Prueba manual en navegador
7. Commit con mensaje descriptivo referenciando el issue

RESTRICCIONES:
- ❌ NO usar Options API (solo Composition API)
- ❌ NO usar inline styles (solo TailwindCSS)
- ✅ <script setup> obligatorio
- ✅ Pinia para state management
- ✅ TDD con Vitest
- ✅ Responsive design

VERIFICACIÓN FINAL:
- [ ] Todos los tests pasan
- [ ] App corre sin errores
- [ ] Funciona en navegador
- [ ] Responsive (mobile + desktop)
- [ ] Commit y push completos

COMENZAR CON ISSUE #<NÚMERO>
```

---

## 🎯 Ejemplos de Uso

### Ejemplo 1: Implementar US-02 (Create Task)

```
Eres un agente especializado en desarrollo backend .NET 8 con Clean Architecture y TDD.

CONTEXTO:
- Proyecto: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: .NET 8, Clean Architecture, MongoDB + PostgreSQL, TDD
- Guía principal: docs/USER_STORIES.md
- Prompt completo: docs/AGENT_PROMPT_BACKEND.md

TAREA:
Implementar Issue #1 (US-02: Create Task) siguiendo estrictamente TDD y Clean Architecture.

INSTRUCCIONES:
1. Lee el issue: gh issue view 1
2. Identifica US-02 en docs/USER_STORIES.md
3. Lee la guía completa en docs/AGENT_PROMPT_BACKEND.md
4. Implementa siguiendo TDD Red-Green-Refactor:
   - Domain: TaskEntity.Create con validaciones
   - Application: CreateTaskCommand + Handler
   - Infrastructure: MongoTaskRepository.CreateAsync
   - WebApi: POST /api/tasks endpoint
5. Verifica:
   - dotnet test
   - curl -X POST http://localhost:5077/api/tasks -d '{"title":"Test",...}'
6. Commit: "feat: Implement US-02 Create Task #1"

COMENZAR CON ISSUE #1
```

### Ejemplo 2: Implementar US-13 (Login Page)

```
Eres un agente especializado en desarrollo frontend Vue.js 3 con Composition API y TDD.

CONTEXTO:
- Proyecto: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: Vue.js 3, Pinia, TailwindCSS, Vitest
- Guía principal: docs/USER_STORIES.md
- Prompt completo: docs/AGENT_PROMPT_FRONTEND.md

TAREA:
Implementar Issue #9 (US-13: Login Page) siguiendo TDD.

INSTRUCCIONES:
1. Lee el issue: gh issue view 9
2. Identifica US-13 en docs/USER_STORIES.md
3. Lee la guía completa en docs/AGENT_PROMPT_FRONTEND.md
4. Implementa siguiendo TDD:
   - Auth Store (Pinia)
   - API Service (login)
   - LoginForm Component
   - LoginView
   - Router con auth guard
5. Verifica:
   - npm run test
   - npm run dev
   - Prueba login en http://localhost:3000
6. Commit: "feat: Implement US-13 Login Page #9"

COMENZAR CON ISSUE #9
```

---

## 📋 Template de Mensaje al Agente

### Para Backend

```
Implementa Issue #X del proyecto BLA Task Management System.

Repo: C:\Users\devke\source\bla-task-management-system

Lee y sigue las instrucciones completas en:
- docs/AGENT_PROMPT_BACKEND.md
- docs/USER_STORIES.md (busca US-XX correspondiente al issue)

Requisitos:
- TDD estricto (Red-Green-Refactor)
- Clean Architecture
- Drivers nativos (MongoDB.Driver, Npgsql)
- Todos los tests pasando
- Verificación manual en Swagger/Postman

Comienza con: gh issue view X
```

### Para Frontend

```
Implementa Issue #X del proyecto BLA Task Management System.

Repo: C:\Users\devke\source\bla-task-management-system

Lee y sigue las instrucciones completas en:
- docs/AGENT_PROMPT_FRONTEND.md
- docs/USER_STORIES.md (busca US-XX correspondiente al issue)

Requisitos:
- TDD con Vitest
- Vue.js 3 Composition API
- Pinia + TailwindCSS
- Responsive design
- Todos los tests pasando

Comienza con: gh issue view X
```

---

## 🔗 Issues y User Stories

### Backend - Tasks API

| Issue | User Story | Descripción |
|-------|------------|-------------|
| #1 | US-02 | Create Task (POST /api/tasks) |
| #2 | US-03 | Update Task (PUT /api/tasks/{id}) |
| #3 | US-05 | Delete Task (DELETE /api/tasks/{id}) |
| #4 | US-06 | Get Task by ID (GET /api/tasks/{id}) |

### Backend - Users API

| Issue | User Story | Descripción |
|-------|------------|-------------|
| #5 | US-07 | User Registration (POST /api/users/register) |
| #6 | US-08 | User Login (POST /api/users/login) |
| #7 | US-09 | Get Current User (GET /api/users/me) |
| #8 | US-10 | Get All Users (GET /api/users) |

### Frontend

| Issue | User Story | Descripción |
|-------|------------|-------------|
| #9 | US-13 | Login Page |
| #10 | US-15 | Task List View |

---

## 🎓 Recursos Disponibles

**Documentación del Proyecto:**
- `docs/USER_STORIES.md` - 17 user stories con guías de implementación
- `docs/AGENT_PROMPT_BACKEND.md` - Guía completa para backend
- `docs/AGENT_PROMPT_FRONTEND.md` - Guía completa para frontend
- `docs/DEVELOPMENT_WORKFLOW.md` - Workflows de desarrollo
- `docs/TESTING_APIS.md` - Cómo probar las APIs

**GitHub:**
- Issues: https://github.com/dkennyq/bla-task-management-system/issues
- Project: https://github.com/users/dkennyq/projects/1

**APIs:**
- Tasks API: http://localhost:5077/swagger
- Users API: http://localhost:5078/swagger (cuando esté implementada)

---

## 💡 Tips para el Usuario

### Cómo Delegar una Tarea

1. **Identifica el issue:**
   ```bash
   gh issue list
   ```

2. **Copia el prompt correspondiente** (backend o frontend)

3. **Personaliza el número de issue:**
   - Reemplaza `<NÚMERO>` con el issue real (ej: 1, 2, 9, etc.)

4. **Pega el prompt al agente** y deja que trabaje

5. **Verifica el resultado:**
   - Backend: `dotnet test` + Swagger/Postman
   - Frontend: `npm run test` + navegador

6. **Aprueba el commit** si todo está correcto

---

## 🚀 Flujo de Trabajo Recomendado

### Backend (Issues #1-8)

```bash
# 1. Asigna issue al agente
"Implementa Issue #1 siguiendo docs/AGENT_PROMPT_BACKEND.md"

# 2. Agente trabaja autónomamente

# 3. Verifica resultado
cd C:\Users\devke\source\bla-task-management-system
dotnet test
# Prueba en http://localhost:5077/swagger

# 4. Si todo bien, aprueba
git log -1  # Ver último commit
# Listo para el siguiente issue
```

### Frontend (Issues #9-10)

```bash
# 1. Asigna issue al agente
"Implementa Issue #9 siguiendo docs/AGENT_PROMPT_FRONTEND.md"

# 2. Agente trabaja autónomamente

# 3. Verifica resultado
cd C:\Users\devke\source\bla-task-management-system\apps\web
npm run test
npm run dev
# Prueba en http://localhost:3000

# 4. Si todo bien, aprueba
git log -1
# Listo para el siguiente issue
```

---

**Última actualización:** 2026-06-09  
**Versión:** 1.0  
**Autor:** BLA Task Management Team
