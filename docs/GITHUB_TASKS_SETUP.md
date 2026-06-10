# GitHub Tasks Setup - BLA Task Management System

## Overview
Este documento explica cómo usar GitHub Issues y GitHub Projects para gestionar las tareas del proyecto de manera similar a Jira.

---

## GitHub Issues vs Jira

GitHub ofrece dos sistemas complementarios:

### 1. **GitHub Issues** (Similar a Jira Issues)
- Cada issue representa una tarea, bug, o user story
- Soporta labels (como tags en Jira)
- Soporta milestones (como sprints en Jira)
- Asignación de responsables
- Comentarios y discusiones
- Referencias cruzadas entre issues

### 2. **GitHub Projects** (Similar a Jira Boards)
- Vista Kanban o Table
- Workflow customizable (To Do, In Progress, Done)
- Automation rules
- Filtros y grouping
- Roadmap view
- Tracking de progress

---

## Estructura Recomendada para este Proyecto

### Labels (Etiquetas)
```
Type:
  - type:feature       (azul)    - Nueva funcionalidad
  - type:bug           (rojo)    - Error o bug
  - type:test          (verde)   - Testing related
  - type:docs          (amarillo) - Documentación
  
Priority:
  - priority:high      (rojo)    - Alta prioridad
  - priority:medium    (naranja) - Media prioridad
  - priority:low       (verde)   - Baja prioridad
  
Layer:
  - layer:domain       (morado)  - Domain layer
  - layer:application  (azul)    - Application layer
  - layer:infrastructure (cyan)  - Infrastructure layer
  - layer:webapi       (verde)   - WebApi layer
  - layer:frontend     (rosa)    - Frontend layer
  
Status:
  - status:blocked     (gris)    - Bloqueado
  - status:in-progress (amarillo) - En progreso
  - status:review      (celeste)  - En revisión
```

### Milestones (Hitos)
```
1. Backend MVP - Tasks API
   Due date: 2026-06-12
   Description: Complete CRUD operations for Tasks API with TDD
   
2. Backend MVP - Users API
   Due date: 2026-06-15
   Description: Complete authentication and user management with TDD
   
3. Frontend MVP
   Due date: 2026-06-18
   Description: Vue.js application with all features
   
4. Integration & Deployment
   Due date: 2026-06-20
   Description: Docker deployment and end-to-end testing
   
5. Documentation & Presentation
   Due date: 2026-06-22
   Description: Complete documentation and presentation preparation
```

---

## Crear Issues desde USER_STORIES.md

### Plantilla para Backend Issues

#### Ejemplo: US-02 Create Task
```markdown
Title: [Backend] US-02: Create Task Endpoint

Labels: type:feature, priority:high, layer:application, layer:infrastructure, layer:webapi

Milestone: Backend MVP - Tasks API

Description:
## User Story
**As a** registered user  
**I want to** create a new task  
**So that I** can keep track of work I need to do

## Technical Specification

### Endpoint
POST /api/tasks

### Request Body
json
{
  "title": "Complete technical interview",
  "description": "Build full-stack application",
  "priority": "High",
  "dueDate": "2026-06-15T00:00:00Z",
  "userId": "00000000-0000-0000-0000-000000000001"
}


### Response
- Status: 201 Created
- Body: TaskEntity object

### Implementation Layers
- [ ] Domain Layer: Add Create validation to TaskEntity
- [ ] Application Layer: CreateTaskCommand + CreateTaskCommandHandler
- [ ] Infrastructure Layer: MongoTaskRepository.CreateAsync implementation
- [ ] WebApi Layer: TasksController POST action

### TDD Requirements
- [ ] Write failing tests first (RED)
- [ ] Implement minimal code (GREEN)
- [ ] Refactor while keeping tests green
- [ ] Target: 100% coverage on business logic

### Acceptance Criteria
- [x] User must provide valid userId (not empty GUID)
- [x] Title is required (max 200 chars)
- [x] Description is optional (max 2000 chars)
- [x] Priority defaults to Medium if not provided
- [x] DueDate is optional
- [x] Status defaults to Pending
- [x] CreatedAt and UpdatedAt set automatically
- [x] Returns 400 BadRequest for validation errors
- [x] Returns 201 Created with created task

### Dependencies
- MongoDB container running
- ITaskRepository interface exists (✅ done)
- TaskEntity exists (✅ done)

### Reference
See: docs/USER_STORIES.md - US-02
```

---

### Plantilla para Frontend Issues

#### Ejemplo: US-13 Login Page
```markdown
Title: [Frontend] US-13: Login Page Component

Labels: type:feature, priority:high, layer:frontend

Milestone: Frontend MVP

Description:
## User Story
**As a** user  
**I want to** a responsive login page  
**So that I** can access the system from any device

## Technical Specification

### Component
`src/views/LoginView.vue`

### Routes
- Path: `/login`
- Name: `Login`

### Features
- Form with username/email and password fields
- "Remember me" checkbox
- "Forgot password?" link (disabled for MVP)
- "Register" link to /register
- Client-side validation
- Loading state during API call
- Error handling with user-friendly messages
- Redirect to /tasks on success

### API Integration
- Endpoint: POST /api/users/login
- Store JWT token in localStorage or sessionStorage
- Set user state in Pinia/Vuex store

### Styling
- Responsive: mobile, tablet, desktop
- Tailwind CSS or CSS modules
- Accessible (ARIA labels, keyboard navigation)

### Implementation Checklist
- [ ] Create LoginView.vue component
- [ ] Create LoginForm.vue child component
- [ ] Add validation composable (useValidation)
- [ ] Add auth composable (useAuth)
- [ ] Configure route in router/index.ts
- [ ] Add unit tests (Vitest)
- [ ] Add E2E tests (optional)

### Acceptance Criteria
- [x] Form submits on Enter key
- [x] Validation errors shown inline
- [x] Loading spinner shown during login
- [x] Success redirects to /tasks
- [x] Error shows toast notification
- [x] Works on mobile, tablet, desktop
- [x] Accessible (keyboard navigation, screen readers)

### Dependencies
- Users API login endpoint (must be implemented first)
- Vue Router configured
- Pinia/Vuex store for auth state

### Reference
See: docs/USER_STORIES.md - US-13
```

---

## Comandos para Crear Issues via GitHub CLI

### Instalación GitHub CLI
```bash
# Windows (Chocolatey)
choco install gh

# Verificar instalación
gh --version

# Autenticar
gh auth login
```

### Crear Issue desde Línea de Comandos
```bash
# Crear issue básico
gh issue create \
  --title "[Backend] US-02: Create Task Endpoint" \
  --label "type:feature,priority:high,layer:application" \
  --milestone "Backend MVP - Tasks API" \
  --body "See template above"

# Crear issue desde archivo
gh issue create --title "[Backend] US-02" --body-file issue-us02.md

# Listar issues
gh issue list

# Ver issue específico
gh issue view 5

# Asignar issue
gh issue edit 5 --add-assignee @me

# Cerrar issue
gh issue close 5
```

---

## Configurar GitHub Project

### Paso 1: Crear Proyecto
1. Ve a tu repositorio en GitHub
2. Click en "Projects" tab
3. Click "New project"
4. Selecciona "Board" template
5. Nombre: "BLA Task Management - Development"

### Paso 2: Configurar Columnas
```
1. 📋 Backlog        - Issues sin asignar
2. 🎯 Ready          - Issues listos para trabajar
3. 🚧 In Progress    - Issues en desarrollo
4. 👀 In Review      - Pull requests en revisión
5. ✅ Done           - Issues completados
6. 🚫 Blocked        - Issues bloqueados
```

### Paso 3: Configurar Automation Rules
```
Rule 1: Auto-add issues
  - When: Issue is created with label "type:feature"
  - Then: Add to "Backlog" column

Rule 2: Auto-move to In Progress
  - When: Issue is assigned
  - Then: Move to "In Progress" column

Rule 3: Auto-move to In Review
  - When: Pull request references issue
  - Then: Move to "In Review" column

Rule 4: Auto-move to Done
  - When: Issue is closed
  - Then: Move to "Done" column
```

### Paso 4: Agregar Issues al Project
```bash
# Via CLI
gh project item-add <project-number> --owner <username> --url <issue-url>

# O desde la UI
# Drag & drop issues desde "Issues" tab al Project board
```

---

## Workflow Recomendado

### 1. Crear Issues desde USER_STORIES.md
```bash
# Crear todas las issues de Tasks API
gh issue create --title "[Backend] US-02: Create Task" --label "type:feature,priority:high" --body-file issues/us-02.md
gh issue create --title "[Backend] US-03: Update Task" --label "type:feature,priority:high" --body-file issues/us-03.md
gh issue create --title "[Backend] US-04: Update Task Status" --label "type:feature,priority:medium" --body-file issues/us-04.md
gh issue create --title "[Backend] US-05: Delete Task" --label "type:feature,priority:high" --body-file issues/us-05.md
gh issue create --title "[Backend] US-06: Get Task by ID" --label "type:feature,priority:medium" --body-file issues/us-06.md
```

### 2. Priorizar en Project Board
- Mover a "Ready" las issues prioritarias
- Asignar milestone correspondiente

### 3. Comenzar Desarrollo
- Asignarse el issue: `gh issue edit <number> --add-assignee @me`
- Mover a "In Progress"
- Crear branch: `git checkout -b feature/us-02-create-task`

### 4. Implementar con TDD
- Escribir tests (RED)
- Implementar código (GREEN)
- Refactor
- Commit frecuentes con referencias: `git commit -m "feat: Add CreateTaskCommandHandler #2"`

### 5. Crear Pull Request
```bash
gh pr create \
  --title "feat: US-02 - Create Task Endpoint" \
  --body "Closes #2" \
  --assignee @me
```

### 6. Merge y Close
- Después del merge, GitHub cierra automáticamente el issue
- El issue se mueve automáticamente a "Done"

---

## Generar Issues Automáticamente desde USER_STORIES.md

### Script PowerShell (crear-issues.ps1)
```powershell
# Script para crear issues desde USER_STORIES.md

$issues = @(
    @{
        title = "[Backend] US-02: Create Task Endpoint"
        labels = "type:feature,priority:high,layer:application"
        milestone = "Backend MVP - Tasks API"
        body = @"
## User Story
As a registered user, I want to create a new task

See: docs/USER_STORIES.md - US-02

## Implementation Layers
- [ ] Domain Layer
- [ ] Application Layer
- [ ] Infrastructure Layer
- [ ] WebApi Layer

## TDD Required
- Write tests first (RED)
- Implement code (GREEN)
- Refactor
"@
    },
    @{
        title = "[Backend] US-03: Update Task Endpoint"
        labels = "type:feature,priority:high,layer:application"
        milestone = "Backend MVP - Tasks API"
        body = @"
## User Story
As a registered user, I want to update task details

See: docs/USER_STORIES.md - US-03

## Implementation Layers
- [ ] Domain Layer
- [ ] Application Layer
- [ ] Infrastructure Layer
- [ ] WebApi Layer
"@
    }
    # ... más issues
)

foreach ($issue in $issues) {
    gh issue create `
        --title $issue.title `
        --label $issue.labels `
        --milestone $issue.milestone `
        --body $issue.body
    
    Write-Host "Created: $($issue.title)" -ForegroundColor Green
}
```

### Ejecutar Script
```powershell
cd C:\Users\devke\source\bla-task-management-system
.\scripts\crear-issues.ps1
```

---

## Integración con AI Agents

### Para Agentes de IA
Los issues creados desde USER_STORIES.md contienen toda la información necesaria:

1. **Context**: User story en formato estándar
2. **Technical Specs**: Endpoint, request/response, layers
3. **Acceptance Criteria**: Checklist claro
4. **Dependencies**: Qué debe existir antes
5. **Reference**: Link al documento completo

Un agente de IA puede:
```
1. Leer el issue
2. Entender el contexto técnico
3. Implementar siguiendo TDD
4. Validar contra acceptance criteria
5. Crear PR con referencia al issue
```

### Ejemplo de Prompt para AI Agent
```
Implementa el issue #2 "[Backend] US-02: Create Task Endpoint"

Contexto:
- Proyecto .NET 8 con Clean Architecture
- MongoDB como database (native driver)
- TDD es obligatorio (RED-GREEN-REFACTOR)
- 17 tests ya pasando en la baseline

Instrucciones:
1. Lee el issue completo en GitHub
2. Implementa siguiendo los 4 layers
3. Escribe tests primero (RED)
4. Implementa código mínimo (GREEN)
5. Refactoriza
6. Verifica todos los acceptance criteria
7. Crea commit con mensaje: "feat: US-02 - Create Task Endpoint #2"

Referencias:
- docs/USER_STORIES.md
- apps/tasks-api/src/ (código existente)
```

---

## Métricas y Tracking

### Dashboard View en GitHub Projects
- **Burndown Chart**: Tracking de issues completados
- **Velocity**: Issues cerrados por milestone
- **Cycle Time**: Tiempo desde "Ready" hasta "Done"
- **WIP Limit**: Máximo de issues "In Progress"

### Reports
```bash
# Issues abiertas por label
gh issue list --label "type:feature" --state open

# Issues cerradas este mes
gh issue list --state closed --search "closed:>2026-06-01"

# Issues por milestone
gh issue list --milestone "Backend MVP - Tasks API"
```

---

## Conclusión

**Sí, GitHub puede manejar tareas al estilo Jira**:
- ✅ Issues = Jira Issues
- ✅ Projects = Jira Boards
- ✅ Milestones = Sprints
- ✅ Labels = Tags/Components
- ✅ Assignees = Responsables
- ✅ PR linking = Issue linking
- ✅ Automation = Workflow rules

**Y puedes crear issues directamente desde USER_STORIES.md**:
- Manual: Copiar/pegar cada US como issue template
- Semi-automático: Script PowerShell con gh CLI
- Automático: GitHub Actions workflow (avanzado)

---

**Next Steps:**
1. Crear labels en el repositorio
2. Crear milestones
3. Generar issues desde USER_STORIES.md (manual o script)
4. Configurar GitHub Project con automation
5. Comenzar desarrollo siguiendo el workflow

¿Te gustaría que cree el script PowerShell completo para generar todos los issues automáticamente?
