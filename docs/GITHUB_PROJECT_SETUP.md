# GitHub Project Setup - Manual Instructions

## 🎯 Objetivo
Configurar un GitHub Project (Kanban Board) para gestionar el desarrollo del proyecto BLA Task Management System.

---

## 📊 Paso 1: Crear el Proyecto

### Opción A: Desde el Repositorio
1. Ve a: https://github.com/dkennyq/bla-task-management-system
2. Click en la pestaña **"Projects"**
3. Click en **"Link a project"** o **"New project"**
4. Si pregunta dónde crear el proyecto, selecciona:
   - **User-level project** (para proyectos personales)
   - O **Repository-level project** (vinculado al repo)

### Opción B: Desde tu Perfil
1. Ve a: https://github.com/dkennyq?tab=projects
2. Click en **"New project"**
3. Selecciona **"Link to repository"** → dkennyq/bla-task-management-system

### Configuración Inicial
- **Nombre**: `BLA Task Management - Development`
- **Template**: **Board** (Kanban view)
- **Descripción**: `Full-stack task management system with Clean Architecture, TDD, and microservices`

---

## 🗂️ Paso 2: Configurar Columnas

El template "Board" viene con columnas básicas. Configúralas así:

### Columnas Recomendadas:
1. **📋 Backlog** (rename "Todo")
   - Issues sin asignar, priorizados
   
2. **🎯 Ready** (nueva)
   - Issues listos para trabajar
   - Tienen todas las dependencias resueltas
   
3. **🚧 In Progress** (ya existe)
   - Issues actualmente en desarrollo
   - WIP limit: 3 issues máximo
   
4. **👀 In Review** (nueva)
   - Pull requests esperando revisión
   
5. **✅ Done** (ya existe)
   - Issues completados y merged

### Cómo Agregar/Renombrar Columnas:
1. Click en el menú **"⋮"** de cada columna
2. Selecciona **"Rename"** o **"Add column"**
3. Drag & drop para reordenar

---

## 🏷️ Paso 3: Agregar Issues al Proyecto

### Método 1: Agregar Todos los Issues (Recomendado)

#### Via GitHub UI:
1. En el proyecto, click **"+"** (Add item)
2. Busca el repositorio: `dkennyq/bla-task-management-system`
3. Selecciona todos los issues (#1 a #12)
4. Click **"Add selected items"**

#### Via GitHub CLI:
```powershell
# Primero, obtén el número de tu proyecto
gh project list --owner dkennyq

# Luego agrega todos los issues (reemplaza <PROJECT_NUMBER> con el número real)
for ($i=1; $i -le 12; $i++) {
    gh project item-add <PROJECT_NUMBER> --owner dkennyq --url "https://github.com/dkennyq/bla-task-management-system/issues/$i"
    Write-Host "Issue #$i agregado"
    Start-Sleep -Milliseconds 300
}
```

### Método 2: Agregar Issues Individualmente
1. En cada issue en GitHub
2. Click **"Projects"** en el sidebar derecho
3. Selecciona tu proyecto
4. Automáticamente se agrega a "Backlog"

---

## ⚙️ Paso 4: Configurar Automation

### Reglas de Automatización Recomendadas:

1. **Auto-add new issues**
   - When: Issue is opened
   - Then: Add to project → "Backlog" column
   
2. **Auto-move to In Progress**
   - When: Issue is assigned
   - Then: Move to "In Progress" column
   
3. **Auto-move to In Review**
   - When: Pull request is linked (references issue)
   - Then: Move to "In Review" column
   
4. **Auto-move to Done**
   - When: Issue is closed
   - Then: Move to "Done" column

### Cómo Configurar:
1. En el proyecto, click **"⋮"** (menú superior derecho)
2. Selecciona **"Workflows"**
3. Para cada workflow:
   - Click **"Edit workflow"**
   - Configura las condiciones (When/Then)
   - Click **"Save"**

---

## 🎨 Paso 5: Personalizar Vistas

### Vista 1: Por Prioridad
1. Click **"View"** → **"New view"**
2. Nombre: "By Priority"
3. Layout: **Table**
4. Group by: **Labels** (priority:high, priority:medium, priority:low)
5. Sort by: **Priority** (high → low)

### Vista 2: Por Layer
1. Click **"View"** → **"New view"**
2. Nombre: "By Architecture Layer"
3. Layout: **Board**
4. Group by: **Labels** (layer:domain, layer:application, layer:infrastructure, layer:webapi, layer:frontend)

### Vista 3: Roadmap (Timeline)
1. Click **"View"** → **"New view"**
2. Nombre: "Roadmap"
3. Layout: **Roadmap**
4. Group by: **Milestone**
5. Esto muestra un gantt chart con los milestones

---

## 📊 Paso 6: Configurar Campos Personalizados (Opcional)

### Agregar Campo: Story Points
1. En el proyecto, click **"⋮"** → **"Settings"**
2. Click **"+ New field"**
3. **Field type**: Number
4. **Field name**: Story Points
5. Asigna puntos a cada issue según complejidad

### Agregar Campo: Sprint
1. **Field type**: Single select
2. **Field name**: Sprint
3. **Options**: Sprint 1, Sprint 2, Sprint 3...

---

## 🚀 Paso 7: Workflow de Trabajo

### Para Desarrolladores:
```
1. Selecciona issue de "Ready" (ya tiene dependencias resueltas)
2. Asígnate el issue → Se mueve automáticamente a "In Progress"
3. Crea branch: git checkout -b feature/us-02-create-task
4. Desarrolla con TDD (RED-GREEN-REFACTOR)
5. Commit con referencia: git commit -m "feat: Add CreateTaskCommand #1"
6. Push y crea PR: gh pr create --title "feat: US-02 Create Task" --body "Closes #1"
7. PR entra a "In Review" automáticamente
8. Después del merge, issue se mueve a "Done"
```

### Para Revisores:
1. Revisa PRs en la columna "In Review"
2. Aprueba o solicita cambios
3. Merge → Issue se cierra automáticamente (si PR dice "Closes #X")

---

## 📈 Paso 8: Métricas y Tracking

### Dashboards Disponibles:
1. **Insights** tab en el proyecto
   - Burn-down chart
   - Velocity
   - Cycle time
   - Lead time

### Filtros Útiles:
```
# Issues de alta prioridad sin asignar
is:open is:issue no:assignee label:priority:high

# Issues bloqueados
is:open is:issue label:status:blocked

# Issues por milestone
is:open is:issue milestone:"Backend MVP - Tasks API"

# Issues en progreso hace más de 3 días
is:open is:issue label:status:in-progress updated:<2026-06-06
```

---

## 🎯 Estado Actual del Proyecto

### ✅ Completado
- 15 labels creados (type, priority, layer, status)
- 5 milestones creados con fechas
- 12 issues creados con detalles completos
- Todos los issues tienen labels y milestones asignados

### 🔲 Pendiente (Manual)
- [ ] Crear el proyecto en GitHub UI
- [ ] Agregar los 12 issues al proyecto
- [ ] Configurar automation rules
- [ ] Crear vistas personalizadas (By Priority, By Layer, Roadmap)

---

## 📚 Referencias

- GitHub Projects Documentation: https://docs.github.com/en/issues/planning-and-tracking-with-projects
- Project Automation: https://docs.github.com/en/issues/planning-and-tracking-with-projects/automating-your-project
- GitHub CLI Projects: https://cli.github.com/manual/gh_project

---

## 💡 Tips

1. **Prioriza visualmente**: Usa el drag & drop para reordenar issues en cada columna
2. **WIP Limits**: Limita "In Progress" a 3 issues para evitar multitasking
3. **Daily Standup**: Usa la vista Board para revisiones rápidas
4. **Sprint Planning**: Usa la vista Roadmap para planificar milestones
5. **Retrospectiva**: Usa Insights para analizar velocity y cycle time

---

**Next Step**: Ve a https://github.com/dkennyq/bla-task-management-system/projects y crea tu proyecto! 🚀
