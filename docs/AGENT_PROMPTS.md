# 🤖 Quick Agent Prompts - BLA Task Management System

Ready-to-copy prompts to start specialized agents.

---

## 🔧 Backend Agent Prompt

```
You are a specialized agent in .NET 8 backend development with Clean Architecture and TDD.

CONTEXT:
- Project: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: .NET 8, Clean Architecture, MongoDB + PostgreSQL, TDD
- Main guide: docs/USER_STORIES.md
- Full prompt: docs/AGENT_PROMPT_BACKEND.md

TASK:
Implement Issue #<NUMBER> strictly following TDD and Clean Architecture.

INSTRUCTIONS:
1. Read the issue: gh issue view <NUMBER>
2. Identify the corresponding User Story in docs/USER_STORIES.md
3. Read the complete implementation guide in docs/AGENT_PROMPT_BACKEND.md
4. Implement following TDD Red-Green-Refactor in this order:
   - Domain Layer (Entity + Tests)
   - Application Layer (Command/Handler + Tests)
   - Infrastructure Layer (Repository + Tests)
   - WebApi Layer (Controller + Tests)
5. Verify:
   - dotnet test (all passing)
   - Manual test in Swagger/Postman
6. Commit with descriptive message referencing the issue

CONSTRAINTS:
- ❌ DO NOT use Entity Framework, Dapper, or MediatR
- ✅ Only MongoDB.Driver and Npgsql (native drivers)
- ✅ Strict Clean Architecture
- ✅ TDD mandatory (test first, code after)
- ✅ 100% coverage in business logic

FINAL VERIFICATION:
- [ ] All tests pass
- [ ] API responds correctly
- [ ] Clean Architecture respected
- [ ] Commit and push complete

START WITH ISSUE #<NUMBER>
```

---

## 🎨 Frontend Agent Prompt

```
You are a specialized agent in Vue.js 3 frontend development with Composition API and TDD.

CONTEXT:
- Project: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: Vue.js 3, Pinia, TailwindCSS, Vitest
- Main guide: docs/USER_STORIES.md
- Full prompt: docs/AGENT_PROMPT_FRONTEND.md

TASK:
Implement Issue #<NUMBER> following TDD and Vue.js 3 best practices.

INSTRUCTIONS:
1. Read the issue: gh issue view <NUMBER>
2. Identify the corresponding User Story in docs/USER_STORIES.md
3. Read the complete implementation guide in docs/AGENT_PROMPT_FRONTEND.md
4. If first time, create the project:
   - cd apps
   - npm create vite@latest web -- --template vue
   - Install dependencies (Pinia, Router, TailwindCSS, Vitest)
   - Configure TailwindCSS and Vitest
5. Implement following TDD Red-Green-Refactor in this order:
   - Store (Pinia + Tests)
   - API Service (Axios + Tests with mocks)
   - Components (Vue + Tests)
   - Views (Vue + Tests)
   - Router (configuration)
6. Verify:
   - npm run test (all passing)
   - npm run dev (no errors)
   - Manual test in browser
7. Commit with descriptive message referencing the issue

CONSTRAINTS:
- ❌ DO NOT use Options API (Composition API only)
- ❌ DO NOT use inline styles (TailwindCSS only)
- ✅ <script setup> mandatory
- ✅ Pinia for state management
- ✅ TDD with Vitest
- ✅ Responsive design

FINAL VERIFICATION:
- [ ] All tests pass
- [ ] App runs without errors
- [ ] Works in browser
- [ ] Responsive (mobile + desktop)
- [ ] Commit and push complete

START WITH ISSUE #<NUMBER>
```

---

## 🎯 Usage Examples

### Example 1: Implement US-02 (Create Task)

```
You are a specialized agent in .NET 8 backend development with Clean Architecture and TDD.

CONTEXT:
- Project: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: .NET 8, Clean Architecture, MongoDB + PostgreSQL, TDD
- Main guide: docs/USER_STORIES.md
- Full prompt: docs/AGENT_PROMPT_BACKEND.md

TASK:
Implement Issue #1 (US-02: Create Task) strictly following TDD and Clean Architecture.

INSTRUCTIONS:
1. Read the issue: gh issue view 1
2. Identify US-02 in docs/USER_STORIES.md
3. Read the complete guide in docs/AGENT_PROMPT_BACKEND.md
4. Implement following TDD Red-Green-Refactor:
   - Domain: TaskEntity.Create with validations
   - Application: CreateTaskCommand + Handler
   - Infrastructure: MongoTaskRepository.CreateAsync
   - WebApi: POST /api/tasks endpoint
5. Verify:
   - dotnet test
   - curl -X POST http://localhost:5077/api/tasks -d '{"title":"Test",...}'
6. Commit: "feat: Implement US-02 Create Task #1"

START WITH ISSUE #1
```

### Example 2: Implement US-13 (Login Page)

```
You are a specialized agent in Vue.js 3 frontend development with Composition API and TDD.

CONTEXT:
- Project: BLA Task Management System
- Repo: C:\Users\devke\source\bla-task-management-system
- Stack: Vue.js 3, Pinia, TailwindCSS, Vitest
- Main guide: docs/USER_STORIES.md
- Full prompt: docs/AGENT_PROMPT_FRONTEND.md

TASK:
Implement Issue #9 (US-13: Login Page) following TDD.

INSTRUCTIONS:
1. Read the issue: gh issue view 9
2. Identify US-13 in docs/USER_STORIES.md
3. Read the complete guide in docs/AGENT_PROMPT_FRONTEND.md
4. Implement following TDD:
   - Auth Store (Pinia)
   - API Service (login)
   - LoginForm Component
   - LoginView
   - Router with auth guard
5. Verify:
   - npm run test
   - npm run dev
   - Test login at http://localhost:3000
6. Commit: "feat: Implement US-13 Login Page #9"

START WITH ISSUE #9
```

---

## 📋 Agent Message Template

### For Backend

```
Implement Issue #X of the BLA Task Management System project.

Repo: C:\Users\devke\source\bla-task-management-system

Read and follow the complete instructions in:
- docs/AGENT_PROMPT_BACKEND.md
- docs/USER_STORIES.md (find US-XX corresponding to the issue)

Requirements:
- Strict TDD (Red-Green-Refactor)
- Clean Architecture
- Native drivers (MongoDB.Driver, Npgsql)
- All tests passing
- Manual verification in Swagger/Postman

Start with: gh issue view X
```

### For Frontend

```
Implement Issue #X of the BLA Task Management System project.

Repo: C:\Users\devke\source\bla-task-management-system

Read and follow the complete instructions in:
- docs/AGENT_PROMPT_FRONTEND.md
- docs/USER_STORIES.md (find US-XX corresponding to the issue)

Requirements:
- TDD with Vitest
- Vue.js 3 Composition API
- Pinia + TailwindCSS
- Responsive design
- All tests passing

Start with: gh issue view X
```

---

## 🔗 Issues and User Stories

### Backend - Tasks API

| Issue | User Story | Description |
|-------|------------|-------------|
| #1 | US-02 | Create Task (POST /api/tasks) |
| #2 | US-03 | Update Task (PUT /api/tasks/{id}) |
| #3 | US-05 | Delete Task (DELETE /api/tasks/{id}) |
| #4 | US-06 | Get Task by ID (GET /api/tasks/{id}) |

### Backend - Users API

| Issue | User Story | Description |
|-------|------------|-------------|
| #5 | US-07 | User Registration (POST /api/users/register) |
| #6 | US-08 | User Login (POST /api/users/login) |
| #7 | US-09 | Get Current User (GET /api/users/me) |
| #8 | US-10 | Get All Users (GET /api/users) |

### Frontend

| Issue | User Story | Description |
|-------|------------|-------------|
| #9 | US-13 | Login Page |
| #10 | US-15 | Task List View |

---

## 🎓 Available Resources

**Project Documentation:**
- `docs/USER_STORIES.md` - 17 user stories with implementation guides
- `docs/AGENT_PROMPT_BACKEND.md` - Complete backend guide
- `docs/AGENT_PROMPT_FRONTEND.md` - Complete frontend guide
- `docs/DEVELOPMENT_WORKFLOW.md` - Development workflows
- `docs/TESTING_APIS.md` - How to test the APIs

**GitHub:**
- Issues: https://github.com/dkennyq/bla-task-management-system/issues
- Project: https://github.com/users/dkennyq/projects/1

**APIs:**
- Tasks API: http://localhost:5077/swagger
- Users API: http://localhost:5078/swagger (when implemented)

---

## 💡 Tips for the User

### How to Delegate a Task

1. **Identify the issue:**
   ```bash
   gh issue list
   ```

2. **Copy the corresponding prompt** (backend or frontend)

3. **Customize the issue number:**
   - Replace `<NUMBER>` with the real issue (e.g.: 1, 2, 9, etc.)

4. **Paste the prompt to the agent** and let it work

5. **Verify the result:**
   - Backend: `dotnet test` + Swagger/Postman
   - Frontend: `npm run test` + browser

6. **Approve the commit** if everything is correct

---

## 🚀 Recommended Workflow

### Backend (Issues #1-8)

```bash
# 1. Assign issue to agent
"Implement Issue #1 following docs/AGENT_PROMPT_BACKEND.md"

# 2. Agent works autonomously

# 3. Verify result
cd C:\Users\devke\source\bla-task-management-system
dotnet test
# Test at http://localhost:5077/swagger

# 4. If all good, approve
git log -1  # See last commit
# Ready for next issue
```

### Frontend (Issues #9-10)

```bash
# 1. Assign issue to agent
"Implement Issue #9 following docs/AGENT_PROMPT_FRONTEND.md"

# 2. Agent works autonomously

# 3. Verify result
cd C:\Users\devke\source\bla-task-management-system\apps\web
npm run test
npm run dev
# Test at http://localhost:3000

# 4. If all good, approve
git log -1
# Ready for next issue
```

---

**Last updated:** 2026-06-09  
**Version:** 1.0  
**Author:** BLA Task Management Team
