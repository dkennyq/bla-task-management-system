# User Stories - BLA Task Management System

**Project**: Technical Interview Exercise - .NET Full Stack Application  
**Date**: June 2026  
**Version**: 1.0  
**Architecture**: Clean Architecture + TDD + Microservices

---

## 🤖 Instructions for AI Agents

This document is structured to enable autonomous implementation by AI agents. Each user story includes:

### What You'll Find in Each User Story:
1. **User Story**: Business context in "As a... I want to... So that..." format
2. **Acceptance Criteria**: Functional requirements that must be met
3. **Technical Details**: API specification (endpoint, request/response, status codes)
4. **Implementation Guide (TDD)**: Step-by-step instructions for each architecture layer
5. **Files to Create/Modify**: Exact file paths you need to work with
6. **Dependencies**: What must exist before you start (with ✅/🔲 status)

### How to Implement a User Story:
```
1. Read the complete user story (don't skip sections)
2. Verify all dependencies are met (check ✅ status)
3. Follow TDD strictly:
   - Write failing tests FIRST (RED phase)
   - Run tests to verify they fail (dotnet test)
   - Implement minimal code to pass tests (GREEN phase)
   - Run tests to verify they pass
   - Refactor if needed while keeping tests green
4. Work layer by layer: Domain → Application → Infrastructure → WebApi
5. Validate against ALL acceptance criteria before marking as done
6. Commit with message format: "feat: US-XX - [description] #issue-number"
```

### Repository Structure Reference:
```
apps/
├── tasks-api/
│   ├── src/
│   │   ├── TasksApi.Domain/          # Business entities, no dependencies
│   │   ├── TasksApi.Application/     # Use cases, depends on Domain
│   │   ├── TasksApi.Infrastructure/  # External services (MongoDB), depends on Application
│   │   └── TasksApi.WebApi/          # Controllers, depends on all layers
│   └── tests/
│       ├── TasksApi.Domain.Tests/
│       ├── TasksApi.Application.Tests/
│       ├── TasksApi.Infrastructure.Tests/
│       └── TasksApi.WebApi.Tests/
└── users-api/
    ├── src/
    │   ├── UsersApi.Domain/          # User entities, authentication logic
    │   ├── UsersApi.Application/     # Auth use cases, JWT generation
    │   ├── UsersApi.Infrastructure/  # PostgreSQL with Npgsql
    │   └── UsersApi.WebApi/          # Auth endpoints
    └── tests/
        └── [same pattern as tasks-api]
```

### Running Tests:
```bash
# All tests in solution
dotnet test

# Specific project tests
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests

# With detailed output
dotnet test --verbosity normal

# Watch mode (re-run on file change)
dotnet watch test --project apps/tasks-api/tests/TasksApi.Domain.Tests
```

### Key Constraints (MUST FOLLOW):
- ❌ **NEVER use**: Entity Framework, Dapper, Mediator
- ✅ **MUST use**: Native drivers (MongoDB.Driver 2.25.0, Npgsql 8.0.3)
- ✅ **MUST follow**: Clean Architecture (Domain → Application → Infrastructure → WebApi)
- ✅ **MUST apply**: TDD (RED-GREEN-REFACTOR)
- ✅ **MUST write**: Unit tests for Domain/Application/WebApi, Integration tests for Infrastructure

### Baseline (Already Implemented):
- ✅ GET /api/tasks?userId={guid} - Fully implemented with 17 tests
- ✅ MongoDB container running on port 27017
- ✅ PostgreSQL container running on port 5432
- ✅ TaskEntity, ITaskRepository, MongoTaskRepository - All exist
- ✅ Clean Architecture structure - Established

### Connection Strings (for reference):
```csharp
// MongoDB (tasks-api)
"mongodb://localhost:27017/tasksdb"

// PostgreSQL (users-api)
"Host=localhost;Port=5432;Database=usersdb;Username=admin;Password=admin123"
```

---

## Table of Contents
1. [Project Vision](#project-vision)
2. [Technical Stack](#technical-stack)
3. [Implemented Features](#implemented-features)
4. [Pending Features](#pending-features)
5. [User Stories - Tasks Management](#user-stories---tasks-management)
6. [User Stories - Users Management](#user-stories---users-management)
7. [Non-Functional Requirements](#non-functional-requirements)
8. [Acceptance Criteria](#acceptance-criteria)

---

## Project Vision

**As a** technical interview candidate  
**I want to** build a full-stack task management system  
**So that I** can demonstrate proficiency in .NET, Clean Architecture, TDD, modern data storage, and full-stack development skills

### Business Context
The system allows users to manage their personal tasks with full CRUD operations while maintaining secure user authentication. The application follows industry best practices including:
- Clean Architecture for maintainability
- Test-Driven Development for reliability
- Microservices for scalability
- Modern native database drivers (MongoDB, PostgreSQL)
- Responsive Vue.js frontend

---

## Technical Stack

### Backend
- **.NET 8.0** (ASP.NET Core Web API)
- **MongoDB** (Tasks storage) - Native MongoDB.Driver 2.25.0
- **PostgreSQL** (Users storage) - Native Npgsql 8.0.3
- **xUnit** (Testing framework)
- **FluentAssertions** (Test assertions)
- **Moq** (Mocking framework)

### Frontend
- **Vue.js 3** (Composition API)
- **TypeScript** (Type safety)
- **Vite** (Build tool)
- **Axios** (HTTP client)
- **Vue Router** (Routing)

### DevOps
- **Docker Compose** (Container orchestration)
- **GitHub** (Version control)
- **Swagger/OpenAPI** (API documentation)

### Constraints
- ❌ **Cannot use**: Entity Framework, Dapper, Mediator
- ✅ **Must use**: Native database drivers (MongoDB.Driver, Npgsql)

---

## Implemented Features

### ✅ Backend Infrastructure (Completed)
- [x] Project structure with Clean Architecture
- [x] Docker Compose configuration (MongoDB + PostgreSQL)
- [x] .NET 8 solution with 10 projects
- [x] Database seeding (5 demo tasks, 3 demo users)

### ✅ Tasks API - GET Endpoint (Completed with TDD)
- [x] **Domain Layer**: TaskEntity with business rules (7 unit tests)
- [x] **Application Layer**: Query handler with ITaskRepository (3 unit tests)
- [x] **Infrastructure Layer**: MongoTaskRepository with native driver (4 integration tests)
- [x] **WebApi Layer**: TasksController with GET endpoint (3 unit tests)
- [x] **Total**: 17 tests passing

**Endpoint**: `GET /api/tasks?userId={guid}`

---

## Pending Features

### 🔲 Tasks API - Complete CRUD Operations
- [ ] POST /api/tasks (Create task)
- [ ] PUT /api/tasks/{id} (Update task)
- [ ] DELETE /api/tasks/{id} (Delete task)
- [ ] GET /api/tasks/{id} (Get task by ID)
- [ ] Unit tests for each operation

### 🔲 Users API - Complete Implementation
- [ ] POST /api/users/register (User registration)
- [ ] POST /api/users/login (User authentication)
- [ ] GET /api/users/me (Get current user - Authorized)
- [ ] GET /api/users (List users - Non-authorized example)
- [ ] JWT token generation and validation
- [ ] BCrypt password hashing
- [ ] Unit tests for all operations

### 🔲 Frontend - Vue.js Application
- [ ] Login/Register pages
- [ ] Task list view (CRUD operations)
- [ ] Task creation form
- [ ] Task editing modal
- [ ] Task deletion confirmation
- [ ] Responsive design
- [ ] User authentication state management
- [ ] API integration with Axios

### 🔲 Integration & Deployment
- [ ] Complete Dockerfile for tasks-api
- [ ] Dockerfile for users-api
- [ ] Dockerfile for web frontend
- [ ] Full docker-compose deployment
- [ ] End-to-end testing

### 🔲 Documentation
- [ ] README with setup instructions
- [ ] API documentation (Swagger)
- [ ] GenAI tools usage documentation
- [ ] Architecture diagrams
- [ ] Presentation slides

---

## User Stories - Tasks Management

### Epic: Task Management

#### US-01: View My Tasks
**As a** registered user  
**I want to** view all my tasks  
**So that I** can see what work I need to complete

**Acceptance Criteria:**
- User must be logged in
- Display all tasks for the authenticated user
- Show task title, description, status, priority, and due date
- Tasks should be sorted by due date (nearest first)
- Empty state shown when no tasks exist

**Technical Details:**
- Endpoint: `GET /api/tasks?userId={guid}`
- Response: Array of TaskEntity objects
- Status: ✅ **IMPLEMENTED**

---

#### US-02: Create a New Task
**As a** registered user  
**I want to** create a new task  
**So that I** can keep track of work I need to do

**Acceptance Criteria:**
- User must be logged in
- Required fields: title (max 200 chars)
- Optional fields: description, priority, due date
- Default status: Pending
- Task is associated with authenticated user
- Validation errors returned for invalid input
- Success message shown after creation

**Technical Details:**
- Endpoint: `POST /api/tasks`
- Request body:
  ```json
  {
    "title": "Complete technical interview",
    "description": "Build full-stack application",
    "priority": "High",
    "dueDate": "2026-06-15T00:00:00Z",
    "userId": "00000000-0000-0000-0000-000000000001"
  }
  ```
- Response: 201 Created with TaskEntity
- Status: 🔲 **PENDING**

**Implementation Guide (TDD):**
1. **Domain Layer** (`TasksApi.Domain`)
   - ✅ TaskEntity already exists
   - Add factory method: `TaskEntity.Create(title, description, priority, dueDate, userId)` if not exists
   - Validation in constructor already handles required fields

2. **Application Layer** (`TasksApi.Application`)
   - Create `Commands/CreateTaskCommand.cs` (record with validation attributes)
   - Create `Commands/CreateTaskCommandHandler.cs`
   - Handler calls `ITaskRepository.CreateAsync(taskEntity)`
   - Return created TaskEntity

3. **Infrastructure Layer** (`TasksApi.Infrastructure`)
   - ✅ MongoTaskRepository.CreateAsync already exists
   - Maps TaskEntity → TaskDocument
   - Inserts into MongoDB tasks collection
   - Maps TaskDocument → TaskEntity for return

4. **WebApi Layer** (`TasksApi.WebApi`)
   - Add POST action to TasksController
   - Validate request body (use [ApiController] auto-validation)
   - Call CreateTaskCommandHandler
   - Return CreatedAtAction with route to GET by ID

5. **Test Strategy**
   - **Domain**: Test TaskEntity.Create validation
   - **Application**: Mock ITaskRepository, test handler logic
   - **Infrastructure**: Integration test against MongoDB container
   - **WebApi**: Mock handler, test controller returns 201/400

**Files to Create/Modify:**
```
apps/tasks-api/src/TasksApi.Application/Commands/CreateTaskCommand.cs
apps/tasks-api/src/TasksApi.Application/Commands/CreateTaskCommandHandler.cs
apps/tasks-api/tests/TasksApi.Application.Tests/Commands/CreateTaskCommandHandlerTests.cs
apps/tasks-api/tests/TasksApi.Infrastructure.Tests/Repositories/MongoTaskRepositoryTests.cs (add test)
apps/tasks-api/src/TasksApi.WebApi/Controllers/TasksController.cs (add POST action)
apps/tasks-api/tests/TasksApi.WebApi.Tests/Controllers/TasksControllerTests.cs (add tests)
```

**Dependencies:**
- MongoDB.Driver (✅ installed)
- ITaskRepository interface (✅ exists)
- TaskEntity (✅ exists)
- MongoDB container (✅ running)

---

#### US-03: Update an Existing Task
**As a** registered user  
**I want to** update task details  
**So that I** can keep information current

**Acceptance Criteria:**
- User must be logged in
- User can only update their own tasks
- Can update: title, description, status, priority, due date
- Validation applied to all fields
- 404 returned if task not found
- 403 returned if task belongs to another user
- Updated timestamp automatically set

**Technical Details:**
- Endpoint: `PUT /api/tasks/{id}`
- Request body: Same as POST
- Response: 200 OK with updated TaskEntity
- Status: 🔲 **PENDING**

**Implementation Guide (TDD):**
1. **Domain Layer**
   - Add `TaskEntity.Update(title, description, priority, dueDate)` method
   - Validates title not empty, sets UpdatedAt timestamp
   - Keep userId and Id immutable

2. **Application Layer**
   - Create `Commands/UpdateTaskCommand.cs` (id, title, desc, priority, dueDate, userId)
   - Create `Commands/UpdateTaskCommandHandler.cs`
   - Handler logic:
     - Call `ITaskRepository.GetByIdAsync(id)`
     - If null, throw NotFoundException
     - If task.UserId != command.UserId, throw ForbiddenException
     - Call `task.Update(...)`
     - Call `ITaskRepository.UpdateAsync(task)`
     - Return updated task

3. **Infrastructure Layer**
   - ✅ MongoTaskRepository.UpdateAsync already exists
   - Uses `ReplaceOneAsync` with filter by Id

4. **WebApi Layer**
   - Add PUT action to TasksController
   - Extract userId from JWT claims (when auth implemented) or query param (MVP)
   - Call UpdateTaskCommandHandler
   - Return 200 OK / 404 Not Found / 403 Forbidden

5. **Test Strategy**
   - **Domain**: Test Update method validates title, sets UpdatedAt
   - **Application**: Test handler throws exceptions for not found/forbidden
   - **Infrastructure**: Integration test updates task in MongoDB
   - **WebApi**: Test controller returns correct status codes

**Files to Create/Modify:**
```
apps/tasks-api/src/TasksApi.Domain/Entities/TaskEntity.cs (add Update method)
apps/tasks-api/tests/TasksApi.Domain.Tests/Entities/TaskTests.cs (add update tests)
apps/tasks-api/src/TasksApi.Application/Commands/UpdateTaskCommand.cs
apps/tasks-api/src/TasksApi.Application/Commands/UpdateTaskCommandHandler.cs
apps/tasks-api/src/TasksApi.Application/Exceptions/NotFoundException.cs
apps/tasks-api/src/TasksApi.Application/Exceptions/ForbiddenException.cs
apps/tasks-api/tests/TasksApi.Application.Tests/Commands/UpdateTaskCommandHandlerTests.cs
apps/tasks-api/tests/TasksApi.Infrastructure.Tests/Repositories/MongoTaskRepositoryTests.cs (add test)
apps/tasks-api/src/TasksApi.WebApi/Controllers/TasksController.cs (add PUT action)
apps/tasks-api/tests/TasksApi.WebApi.Tests/Controllers/TasksControllerTests.cs (add tests)
```

---

#### US-04: Update Task Status
**As a** registered user  
**I want to** change a task's status  
**So that I** can mark tasks as in progress or completed

**Acceptance Criteria:**
- User must be logged in
- Valid statuses: Pending, InProgress, Completed
- Only task owner can update status
- Updated timestamp automatically set
- UI provides dropdown or buttons for status change

**Technical Details:**
- Endpoint: `PATCH /api/tasks/{id}/status`
- Request body: `{ "status": "Completed" }`
- Response: 200 OK with updated TaskEntity
- Status: 🔲 **PENDING**

---

#### US-05: Delete a Task
**As a** registered user  
**I want to** delete a task  
**So that I** can remove tasks I no longer need

**Acceptance Criteria:**
- User must be logged in
- User can only delete their own tasks
- Confirmation required before deletion
- 404 returned if task not found
- 403 returned if task belongs to another user
- Task permanently removed from database

**Technical Details:**
- Endpoint: `DELETE /api/tasks/{id}`
- Response: 204 No Content
- Status: ✅ **COMPLETED**

---

#### US-06: View Task Details
**As a** registered user  
**I want to** view full details of a specific task  
**So that I** can see all information about the task

**Acceptance Criteria:**
- User must be logged in
- Show all task fields
- Display creation and last updated timestamps
- 404 returned if task not found
- User can only view their own tasks

**Technical Details:**
- Endpoint: `GET /api/tasks/{id}`
- Response: 200 OK with TaskEntity
- Status: 🔲 **PENDING**

---

## User Stories - Users Management

### Epic: User Authentication & Authorization

#### US-07: User Registration
**As a** new user  
**I want to** create an account  
**So that I** can access the task management system

**Acceptance Criteria:**
- Required fields: username, email, password
- Username must be unique (3-50 chars)
- Email must be valid format and unique
- Password must meet complexity requirements:
  - Minimum 8 characters
  - At least 1 uppercase letter
  - At least 1 lowercase letter
  - At least 1 number
  - At least 1 special character
- Password hashed with BCrypt before storage
- Validation errors returned with clear messages
- Success returns user object (without password)

**Technical Details:**
- Endpoint: `POST /api/users/register`
- Request body:
  ```json
  {
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "SecurePass123!",
    "fullName": "John Doe"
  }
  ```
- Response: 201 Created with user object
- Database: PostgreSQL users table
- Status: 🔲 **PENDING**

**Implementation Guide (TDD):**
1. **Domain Layer** (`UsersApi.Domain`)
   - Create `Entities/UserEntity.cs`
     - Properties: Id (Guid), Username, Email, PasswordHash, FullName, CreatedAt
     - Constructor validates username (3-50 chars), email format
     - Static method: `UserEntity.Create(username, email, password, fullName)`
     - Uses BCrypt.Net-Next for hashing
   - Create `ValueObjects/Email.cs` (validation logic)
   - Create `ValueObjects/Username.cs` (validation logic)

2. **Application Layer** (`UsersApi.Application`)
   - Create `Interfaces/IUserRepository.cs`
     - Methods: CreateAsync, GetByUsernameAsync, GetByEmailAsync, ExistsAsync
   - Create `Commands/RegisterUserCommand.cs`
   - Create `Commands/RegisterUserCommandHandler.cs`
     - Check username/email uniqueness (throw ConflictException if exists)
     - Validate password complexity
     - Hash password with BCrypt (workFactor: 12)
     - Call repository.CreateAsync
     - Return UserDto (no password)
   - Create `DTOs/UserDto.cs` (for responses without password)

3. **Infrastructure Layer** (`UsersApi.Infrastructure`)
   - Install Npgsql 8.0.3
   - Create `Models/UserRecord.cs` (for mapping to/from DB)
   - Create `Repositories/PostgresUserRepository.cs`
     - Connection string from configuration
     - CreateAsync: INSERT INTO users
     - GetByUsernameAsync: SELECT with WHERE username = @username
     - Use parameterized queries (SQL injection prevention)
     - Map UserRecord ↔ UserEntity

4. **WebApi Layer** (`UsersApi.WebApi`)
   - Create `Controllers/UsersController.cs`
   - Add POST /api/users/register action
   - Validate request with [ApiController] attributes
   - Call RegisterUserCommandHandler
   - Return 201 Created / 400 Bad Request / 409 Conflict

5. **Test Strategy**
   - **Domain**: Test UserEntity.Create, email/username validation, password hashing
   - **Application**: Mock IUserRepository, test uniqueness checks, password complexity
   - **Infrastructure**: Integration test against PostgreSQL container
   - **WebApi**: Mock handler, test controller status codes

**Database Schema:**
```sql
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_email ON users(email);
```

**Files to Create:**
```
apps/users-api/src/UsersApi.Domain/Entities/UserEntity.cs
apps/users-api/src/UsersApi.Domain/ValueObjects/Email.cs
apps/users-api/src/UsersApi.Domain/ValueObjects/Username.cs
apps/users-api/tests/UsersApi.Domain.Tests/Entities/UserEntityTests.cs

apps/users-api/src/UsersApi.Application/Interfaces/IUserRepository.cs
apps/users-api/src/UsersApi.Application/Commands/RegisterUserCommand.cs
apps/users-api/src/UsersApi.Application/Commands/RegisterUserCommandHandler.cs
apps/users-api/src/UsersApi.Application/DTOs/UserDto.cs
apps/users-api/src/UsersApi.Application/Exceptions/ConflictException.cs
apps/users-api/tests/UsersApi.Application.Tests/Commands/RegisterUserCommandHandlerTests.cs

apps/users-api/src/UsersApi.Infrastructure/Models/UserRecord.cs
apps/users-api/src/UsersApi.Infrastructure/Repositories/PostgresUserRepository.cs
apps/users-api/tests/UsersApi.Infrastructure.Tests/Repositories/PostgresUserRepositoryTests.cs

apps/users-api/src/UsersApi.WebApi/Controllers/UsersController.cs
apps/users-api/src/UsersApi.WebApi/Program.cs
apps/users-api/tests/UsersApi.WebApi.Tests/Controllers/UsersControllerTests.cs
```

**Dependencies:**
- Npgsql 8.0.3 (PostgreSQL native driver)
- BCrypt.Net-Next 4.0.3 (password hashing)
- PostgreSQL container (✅ running on port 5432)

---

#### US-08: User Login
**As a** registered user  
**I want to** log into the system  
**So that I** can access my tasks

**Acceptance Criteria:**
- Login with username or email + password
- Validate credentials against database
- Compare hashed password using BCrypt
- Generate JWT token on successful login
- Token contains: userId, username, email, expiration
- Token expiration: 24 hours
- Return 401 Unauthorized for invalid credentials
- Store refresh token in database

**Technical Details:**
- Endpoint: `POST /api/users/login`
- Request body:
  ```json
  {
    "username": "johndoe",
    "password": "SecurePass123!"
  }
  ```
- Response: 200 OK
  ```json
  {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "7a8f3d2e...",
    "expiresIn": 86400,
    "user": {
      "id": "uuid",
      "username": "johndoe",
      "email": "john.doe@example.com",
      "fullName": "John Doe"
    }
  }
  ```
- Status: 🔲 **PENDING**

---

#### US-09: Get Current User Profile (Authorized)
**As a** logged-in user  
**I want to** view my profile information  
**So that I** can verify my account details

**Acceptance Criteria:**
- Requires valid JWT token in Authorization header
- Returns current user information (no password)
- 401 returned if token missing or invalid
- 403 returned if token expired

**Technical Details:**
- Endpoint: `GET /api/users/me`
- Authorization: Bearer {token}
- Response: 200 OK with user object
- **This is an AUTHORIZED endpoint** (requires authentication)
- Status: 🔲 **PENDING**

---

#### US-10: List All Users (Non-Authorized Example)
**As a** visitor or logged-in user  
**I want to** see a list of registered users  
**So that I** can demonstrate a non-authorized endpoint

**Acceptance Criteria:**
- No authentication required (public endpoint)
- Returns list of users (username, full name only)
- No sensitive information exposed (no emails, no passwords)
- Pagination supported (page, pageSize parameters)
- Maximum 50 users per page

**Technical Details:**
- Endpoint: `GET /api/users?page=1&pageSize=10`
- No Authorization header required
- Response: 200 OK with paginated user list
- **This is a NON-AUTHORIZED endpoint** (no auth required)
- Status: 🔲 **PENDING**

---

#### US-11: Refresh Access Token
**As a** logged-in user  
**I want to** refresh my access token  
**So that I** can continue using the app without logging in again

**Acceptance Criteria:**
- Requires valid refresh token
- Generates new access token
- Validates refresh token against database
- Refresh token is single-use (invalidated after use)
- New refresh token issued with new access token
- 401 returned if refresh token invalid or expired

**Technical Details:**
- Endpoint: `POST /api/users/refresh`
- Request body: `{ "refreshToken": "..." }`
- Response: 200 OK with new tokens
- Status: 🔲 **PENDING**

---

#### US-12: User Logout
**As a** logged-in user  
**I want to** log out of the system  
**So that I** can secure my account

**Acceptance Criteria:**
- Requires valid JWT token
- Invalidate refresh token in database
- Client should discard access token
- 204 No Content returned

**Technical Details:**
- Endpoint: `POST /api/users/logout`
- Authorization: Bearer {token}
- Response: 204 No Content
- Status: 🔲 **PENDING**

---

## User Stories - Frontend

### Epic: Web Interface

#### US-13: User Interface - Login Page
**As a** user  
**I want to** a responsive login page  
**So that I** can access the system from any device

**Acceptance Criteria:**
- Responsive design (mobile, tablet, desktop)
- Form fields: username/email, password
- "Remember me" checkbox
- "Forgot password?" link (can be disabled/placeholder)
- "Register" link to registration page
- Client-side validation with error messages
- Loading state during authentication
- Redirect to tasks page on success

**Status**: 🔲 **PENDING**

---

#### US-14: User Interface - Registration Page
**As a** new user  
**I want to** a registration form  
**So that I** can create an account

**Acceptance Criteria:**
- Responsive design (mobile, tablet, desktop)
- Form fields: username, email, password, confirm password, full name
- Real-time client-side validation
  - Username: 3-50 characters, alphanumeric + underscore
  - Email: valid format
  - Password: 8+ chars, uppercase, lowercase, number, special char
  - Confirm password: matches password
  - Full name: 2-100 characters
- Password strength indicator (weak/medium/strong)
- Terms and conditions checkbox (required)
- "Already have an account? Login" link
- Client-side validation with error messages
- Loading state during registration
- Success message and auto-redirect to login page (2 seconds)
- Error handling: 400 Bad Request, 409 Conflict (username/email exists)

**Technical Details:**
- Component: `apps/web/src/views/RegisterView.vue`
- Route: `/register` (public, unauthenticated)
- API: `POST /api/users/register` (Users API - port 5078)
- Request body:
  ```json
  {
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "SecurePass123!",
    "fullName": "John Doe"
  }
  ```
- Response (201 Created):
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "johndoe",
    "email": "john.doe@example.com",
    "fullName": "John Doe",
    "createdAt": "2026-06-10T12:00:00Z"
  }
  ```
- Response (409 Conflict):
  ```json
  {
    "error": "Username already exists"
  }
  ```

**Implementation Guide (TDD):**

1. **Setup Component Structure**
   - Create `apps/web/src/views/RegisterView.vue` with Composition API
   - Create `apps/web/src/components/auth/RegisterForm.vue` (optional, for reusability)
   - Use Pinia store: `useAuthStore()` for registration logic

2. **Form Validation (Composable)**
   - Create `apps/web/src/composables/useRegisterForm.ts`
   - Implement validation rules with VeeValidate or custom logic
   - Password strength calculation function
   - Form state management (reactive form data, errors, loading)

3. **API Integration**
   - Use `apps/web/src/services/api.js` (axios instance)
   - Create `registerUser(userData)` function
   - Handle 201 Created (success)
   - Handle 400 Bad Request (validation errors)
   - Handle 409 Conflict (duplicate username/email)

4. **UI Components**
   - Text inputs with validation feedback (red border, error message)
   - Password input with toggle visibility button
   - Password strength indicator (progress bar with colors)
   - Checkbox for terms and conditions
   - Submit button with loading spinner
   - Success notification (toast or alert)
   - Link to login page

5. **Routing**
   - Update `apps/web/src/router/index.js`
   - Add `/register` route (public, no auth required)
   - Redirect to `/login` after successful registration

6. **Test Strategy**
   - **Unit Tests** (Vitest):
     - Test form validation rules
     - Test password strength calculation
     - Test error handling
     - Mock API calls
   - **Component Tests**:
     - Test form submission
     - Test validation feedback
     - Test success redirect
     - Test error messages display

**Files to Create:**
```
apps/web/src/views/RegisterView.vue
apps/web/src/composables/useRegisterForm.ts (optional)
apps/web/src/components/auth/RegisterForm.vue (optional)
apps/web/tests/views/RegisterView.spec.js
apps/web/tests/composables/useRegisterForm.spec.js
```

**Files to Modify:**
```
apps/web/src/router/index.js (add /register route)
apps/web/src/services/api.js (add registerUser function)
apps/web/src/stores/auth.js (add register action)
```

**Dependencies:**
- ✅ Users API - POST /api/users/register (US-07 - backend implemented)
- 🔲 Vue.js 3 project setup
- 🔲 Vue Router configured
- 🔲 Pinia store for auth
- 🔲 Axios configured
- 🔲 TailwindCSS for styling
- 🔲 Vitest for testing

**UI/UX Considerations:**
- Auto-focus on username field when page loads
- Tab order: username → email → password → confirm password → full name → checkbox → submit
- Enter key submits form (if valid)
- Show password strength indicator as user types
- Clear error messages on field change
- Disable submit button during loading
- Show success message before redirect

**Status**: 🔲 **PENDING** (blocked by Issue #19 - API Security)

---

#### US-15: User Interface - Task List View
**As a** logged-in user  
**I want to** view my tasks in a clean interface  
**So that I** can manage my work efficiently

**Acceptance Criteria:**
- Responsive design
- Display tasks in cards or table
- Show: title, status badge, priority indicator, due date
- Filter by status (All, Pending, In Progress, Completed)
- Sort by: due date, priority, title
- Search by title or description
- "Create New Task" button
- Empty state with call-to-action

**Status**: 🔲 **PENDING**

---

#### US-16: User Interface - Task Creation/Edit Form
**As a** logged-in user  
**I want to** a form to create/edit tasks  
**So that I** can add or modify task information

**Acceptance Criteria:**
- Modal dialog for both create and edit modes
- Form fields:
  - Title (text input, required, max 200 chars)
  - Description (textarea, optional, max 1000 chars)
  - Priority (dropdown: Low/Medium/High, required)
  - Due Date (date picker, optional)
  - Status (dropdown: Pending/InProgress/Completed, only in edit mode)
- Client-side validation with error messages
- Cancel and Save buttons
- Loading state during save (disabled inputs, spinner on button)
- Success notification (toast)
- Close modal after successful save
- Error handling: 400 Bad Request, 404 Not Found (edit mode)
- Pre-fill form with existing data in edit mode
- Clear form when switching from edit to create mode

**Technical Details:**

**CREATE MODE:**
- Component: `apps/web/src/components/tasks/TaskFormModal.vue`
- Trigger: Click "Create New Task" button in Task List View
- API: `POST /api/tasks` (Tasks API - port 5077)
- Request body:
  ```json
  {
    "title": "Complete project documentation",
    "description": "Write comprehensive documentation for the project",
    "priority": "High",
    "dueDate": "2026-06-15T00:00:00Z",
    "userId": "extracted-from-jwt-token"
  }
  ```
- Response (201 Created):
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Complete project documentation",
    "description": "Write comprehensive documentation for the project",
    "status": "Pending",
    "priority": "High",
    "dueDate": "2026-06-15T00:00:00Z",
    "userId": "user-id-here",
    "createdAt": "2026-06-10T12:00:00Z",
    "updatedAt": "2026-06-10T12:00:00Z"
  }
  ```

**EDIT MODE:**
- API: `PUT /api/tasks/{id}` (Tasks API - port 5077)
- Request body:
  ```json
  {
    "title": "Complete project documentation (Updated)",
    "description": "Write comprehensive documentation for the project including API docs",
    "status": "InProgress",
    "priority": "High",
    "dueDate": "2026-06-16T00:00:00Z",
    "userId": "extracted-from-jwt-token"
  }
  ```
- Response (200 OK): Updated TaskEntity
- Response (404 Not Found):
  ```json
  {
    "error": "Task not found"
  }
  ```

**Implementation Guide (TDD):**

1. **Setup Component Structure**
   - Create `TaskFormModal.vue` with Composition API
   - Props: `isOpen` (boolean), `mode` ('create' | 'edit'), `taskData` (optional)
   - Emit events: `close`, `save`

2. **Form State Management**
   - Create `composables/useTaskForm.ts` (optional)
   - Reactive form data: title, description, priority, dueDate, status
   - Validation rules for each field
   - Form submission handler
   - Reset form function

3. **API Integration**
   - Update `services/api.js`:
     - `createTask(taskData)` → POST /api/tasks
     - `updateTask(id, taskData)` → PUT /api/tasks/{id}
   - Update `stores/tasksStore.js`:
     - `createTask` action
     - `updateTask` action
   - Extract userId from JWT token (auth store)

4. **UI Components**
   - Modal backdrop and container
   - Form header (title: "Create Task" or "Edit Task")
   - Input fields with validation feedback
   - Priority dropdown (Low/Medium/High)
   - Status dropdown (only in edit mode)
   - Date picker for due date
   - Cancel and Save buttons
   - Loading spinner overlay

5. **Validation Rules**
   - Title: Required, 1-200 characters
   - Description: Optional, max 1000 characters
   - Priority: Required, one of: Low, Medium, High
   - Due Date: Optional, must be future date (or today)
   - Status: Required in edit mode, one of: Pending, InProgress, Completed

6. **Test Strategy**
   - **Unit Tests**:
     - Test validation rules
     - Test form state management
     - Test API call formatting
   - **Component Tests**:
     - Test create mode (empty form)
     - Test edit mode (pre-filled form)
     - Test form submission (success)
     - Test error handling (400, 404)
     - Test form reset
     - Test cancel button

**Files to Create:**
```
apps/web/src/components/tasks/TaskFormModal.vue
apps/web/src/composables/useTaskForm.ts (optional)
apps/web/tests/components/tasks/TaskFormModal.spec.js
```

**Files to Modify:**
```
apps/web/src/services/api.js (add createTask, updateTask)
apps/web/src/stores/tasksStore.js (add createTask, updateTask actions)
apps/web/src/views/TasksView.vue (integrate modal)
```

**Dependencies:**
- ✅ POST /api/tasks (US-02 - backend implemented)
- ✅ PUT /api/tasks/{id} (US-04 - backend implemented)
- 🔲 Issue #19 completed (JWT authentication)
- 🔲 Issue #10 completed (Task List View)
- 🔲 Vue.js 3 project setup
- 🔲 Date picker library (e.g., VueDatePicker or native input[type="date"])

**UI/UX Considerations:**
- Modal should close on ESC key or backdrop click
- Auto-focus on title field when modal opens
- Tab order: title → description → priority → due date → status → cancel → save
- Enter key submits form (if focus is not on textarea)
- Show asterisk (*) on required fields
- Disable save button if form is invalid
- Show loading spinner on save button during submission
- Display success toast: "Task created successfully" or "Task updated successfully"
- On success, close modal and refresh task list

**Status**: 🔲 **PENDING** (blocked by Issue #19 - API Security)

---

#### US-17: User Interface - Task Actions (Delete & Status Update)
**As a** logged-in user  
**I want to** quick actions on tasks  
**So that I** can manage tasks efficiently

**Acceptance Criteria:**
- Edit button on each task card/row → Opens Task Form Modal in edit mode
- Delete button on each task card/row → Shows confirmation dialog
- Confirmation dialog:
  - Message: "Are you sure you want to delete '{taskTitle}'? This action cannot be undone."
  - Cancel and Delete buttons
  - Close on ESC or backdrop click
- Status change dropdown (inline on task card/row)
  - Options: Pending, In Progress, Completed
  - Visual feedback on change (optimistic UI update)
  - Revert on API error
- Visual feedback for all actions (loading indicators)
- Optimistic UI updates (instant feedback)
- Error handling with rollback (show error, revert UI)
- Success notifications (toast): "Task deleted successfully", "Status updated successfully"

**Technical Details:**

**DELETE ACTION:**
- API: `DELETE /api/tasks/{id}` (Tasks API - port 5077)
- Response (204 No Content): Success, no body
- Response (404 Not Found):
  ```json
  {
    "error": "Task not found"
  }
  ```
- Response (403 Forbidden): User doesn't own this task

**STATUS UPDATE ACTION:**
- API: `PUT /api/tasks/{id}` (same as full update, but only changing status)
- Request body:
  ```json
  {
    "title": "existing-title",
    "description": "existing-description",
    "status": "InProgress",
    "priority": "existing-priority",
    "dueDate": "existing-due-date",
    "userId": "extracted-from-jwt-token"
  }
  ```
- Response (200 OK): Updated TaskEntity

**Implementation Guide (TDD):**

1. **Setup Action Components**
   - Create `components/tasks/TaskActions.vue`
     - Props: `task` (TaskEntity object)
     - Emit events: `edit`, `delete`, `statusChange`
   - Create `components/tasks/DeleteConfirmDialog.vue`
     - Props: `isOpen` (boolean), `taskTitle` (string)
     - Emit events: `confirm`, `cancel`

2. **Delete Functionality**
   - Update `services/api.js`:
     - `deleteTask(id)` → DELETE /api/tasks/{id}
   - Update `stores/tasksStore.js`:
     - `deleteTask` action
     - Optimistic UI: Remove from state immediately
     - On error: Re-add to state, show error notification
   - Handle confirmation dialog state

3. **Status Update Functionality**
   - Use existing `updateTask` from US-16
   - Optimistic UI: Update status in state immediately
   - On error: Revert to original status, show error notification
   - Show loading indicator on dropdown during API call

4. **UI Components**
   - Action buttons (icon buttons or dropdown menu):
     - Edit button (pencil icon)
     - Delete button (trash icon)
   - Status dropdown (inline select or custom dropdown)
   - Confirmation dialog:
     - Modal backdrop
     - Dialog box with message
     - Cancel (secondary) and Delete (danger) buttons
   - Loading indicators (spinners on buttons)

5. **Optimistic UI Pattern**
   - **On action trigger**:
     1. Update UI immediately (remove item, change status)
     2. Make API call
     3. On success: Keep UI as is, show success toast
     4. On error: Revert UI, show error toast
   - **Benefits**: Feels instant, better UX

6. **Test Strategy**
   - **Unit Tests**:
     - Test delete action (success, 404, error)
     - Test status update (success, error)
     - Test optimistic UI rollback logic
   - **Component Tests**:
     - Test delete button opens confirmation
     - Test confirmation dialog cancel
     - Test confirmation dialog confirm (calls API)
     - Test edit button emits event
     - Test status dropdown change
     - Test loading states
     - Test error handling

**Files to Create:**
```
apps/web/src/components/tasks/TaskActions.vue
apps/web/src/components/tasks/DeleteConfirmDialog.vue
apps/web/tests/components/tasks/TaskActions.spec.js
apps/web/tests/components/tasks/DeleteConfirmDialog.spec.js
```

**Files to Modify:**
```
apps/web/src/services/api.js (add deleteTask function)
apps/web/src/stores/tasksStore.js (add deleteTask action, update status logic)
apps/web/src/views/TasksView.vue (integrate TaskActions component)
apps/web/src/components/tasks/TaskCard.vue (add TaskActions to each card)
```

**Dependencies:**
- ✅ DELETE /api/tasks/{id} (US-05 - backend implemented)
- ✅ PUT /api/tasks/{id} (US-04 - backend implemented)
- 🔲 Issue #19 completed (JWT authentication)
- 🔲 Issue #10 completed (Task List View with task cards)
- 🔲 Issue US-16 completed (Task Form Modal for edit action)

**UI/UX Considerations:**
- Action buttons should be visible on hover (desktop) or always visible (mobile)
- Use icon buttons with tooltips for better UX
- Delete button should be in danger color (red)
- Confirmation dialog should have clear, non-technical language
- Optimistic UI makes app feel faster
- Show undo option after delete (optional, advanced)
- Disable all actions during loading
- Status dropdown should match task card status badge colors
- Consider using dropdown menu (three dots) to group actions on mobile

**Keyboard Shortcuts (Optional):**
- `E` key: Edit selected task
- `Delete` key: Delete selected task (with confirmation)
- `Enter` in confirmation: Confirm delete
- `ESC` in confirmation: Cancel

**Status**: 🔲 **PENDING** (blocked by Issue #19 - API Security)

---

## Non-Functional Requirements

### NFR-01: Clean Architecture
- ✅ Domain layer: Pure business entities (no dependencies)
- ✅ Application layer: Use cases and interfaces (depends on Domain)
- ✅ Infrastructure layer: External services (depends on Application)
- ✅ WebApi layer: HTTP/Controllers (depends on all layers)
- **Status**: Implemented for Tasks API GET

### NFR-02: Test-Driven Development (TDD)
- ✅ Write tests FIRST (RED)
- ✅ Implement minimal code to pass (GREEN)
- ♻️ Refactor while keeping tests green
- ✅ 17/17 tests passing on implemented features
- 🔲 Target: 100% test coverage on business logic

### NFR-03: Modern Data Storage
- ✅ MongoDB for tasks (native MongoDB.Driver)
- ✅ PostgreSQL for users (native Npgsql)
- ❌ No Entity Framework
- ❌ No Dapper
- ❌ No Mediator

### NFR-04: Code Quality
- Readable, well-organized code
- Consistent naming conventions
- XML documentation on public APIs
- No compiler warnings
- Follow C# coding conventions

### NFR-05: Security
- Passwords hashed with BCrypt
- JWT tokens for authentication
- HTTPS in production
- CORS configured appropriately
- Input validation on all endpoints
- SQL injection prevention (parameterized queries)

### NFR-06: Performance
- MongoDB indexes on frequently queried fields (userId)
- JWT token caching
- Efficient database queries
- Response time < 500ms for simple queries

### NFR-07: Documentation
- README with setup instructions
- Swagger/OpenAPI for all endpoints
- Architecture diagrams
- GenAI tools usage documentation
- Seeded demo data with credentials

---

## Acceptance Criteria

### Definition of Done
A user story is considered DONE when:
1. ✅ Code written following TDD (RED-GREEN-REFACTOR)
2. ✅ Unit tests passing (>80% coverage)
3. ✅ Integration tests passing (for Infrastructure layer)
4. ✅ Code reviewed and meets quality standards
5. ✅ API documented in Swagger
6. ✅ Committed to Git with meaningful messages
7. ✅ Deployed to Docker environment
8. ✅ Manually tested end-to-end
9. ✅ Documentation updated

### Project Completion Criteria
The project is complete when:
- ✅ All user stories implemented
- ✅ All tests passing
- ✅ Frontend integrated with backend
- ✅ Docker Compose fully functional
- ✅ README with setup instructions
- ✅ Seeded demo data working
- ✅ Presentation prepared
- ✅ GenAI tools documentation complete

---

## Demo Credentials

### Seeded Users
```
User 1:
  Username: admin
  Email: admin@example.com
  Password: Admin123!
  
User 2:
  Username: johndoe
  Email: john.doe@example.com
  Password: Password123!
  
User 3:
  Username: janedoe
  Email: jane.doe@example.com
  Password: Password123!
```

### Seeded Tasks
- 5 tasks for User 1 (admin)
- Various statuses: Pending, InProgress, Completed
- Various priorities: Low, Medium, High
- Due dates ranging from past due to future

---

## GenAI Tools Usage

### Prompt Engineering Strategy
This project demonstrates effective use of **GitHub Copilot** as the primary GenAI tool:

#### Initial Prompt (Project Structure):
```
Vamos a iniciar un proyecto nuevo. El proyecto estará alineado a los 
últimos estándares de programación backend, frontend. Tendrá carpetas 
para proyectos de backend y carpetas para proyectos de frontend. 
El main stack será .NET (Api, asp.net mvc, web api), Vue para UI.
El documento menciona Base de datos, Clean Architecture, TDD.
No usar EF, Dapper o Mediator. Genera un plan inicial.
```

#### TDD Implementation Prompts:
- Requested strict TDD methodology (RED-GREEN-REFACTOR cycles)
- Prompted for test-first development at each layer
- Validated AI suggestions by running tests (exit code verification)
- Corrected naming conflicts (TaskStatus → TaskEntityStatus)

#### Critical Thinking Applied:
1. **Validation**: Ran all tests to verify AI-generated code
2. **Edge Cases**: Added validation for empty GUIDs, null checks
3. **Architecture**: Ensured dependency rules followed (Domain → Application → Infrastructure)
4. **Best Practices**: Used async/await, CancellationToken, proper HTTP status codes

---

## Presentation Outline

### 1. Project Overview (5 min)
- Problem statement and solution
- Technical stack and architecture
- Demo credentials

### 2. Clean Architecture (5 min)
- Layer separation and dependencies
- TDD implementation (RED-GREEN-REFACTOR)
- 17 tests passing demonstration

### 3. Backend Deep Dive (10 min)
- Tasks API (MongoDB native driver)
- Users API (PostgreSQL native driver)
- Swagger documentation walkthrough
- Code review: Repository pattern, validation, error handling

### 4. Frontend Demonstration (5 min)
- Vue.js application walkthrough
- CRUD operations demonstration
- Responsive design showcase

### 5. GenAI Tools (5 min)
- Prompt engineering examples
- Code validation process
- Corrections and improvements made

### 6. Q&A (10 min)

---

## Appendix: Technical Decisions Log

### Decision 1: Microservices vs Monolith
- **Choice**: Microservices (separate APIs for Tasks and Users)
- **Rationale**: Demonstrates scalability, allows different databases per domain
- **Trade-off**: More complex deployment, but more realistic architecture

### Decision 2: MongoDB for Tasks
- **Choice**: MongoDB (document database)
- **Rationale**: Tasks are self-contained documents, flexible schema
- **Alternative**: PostgreSQL (would also work, but less differentiation)

### Decision 3: PostgreSQL for Users
- **Choice**: PostgreSQL (relational database)
- **Rationale**: Users and authentication fit relational model, ACID compliance
- **Implementation**: Native Npgsql driver, no ORM

### Decision 4: TDD Strategy
- **Choice**: Strict RED-GREEN-REFACTOR
- **Rationale**: Demonstrates discipline, ensures test coverage
- **Evidence**: 17/17 tests passing, test-first commit history

### Decision 5: Integration Tests for Infrastructure
- **Choice**: Real MongoDB container vs mocked
- **Rationale**: Validates actual database interactions, catches serialization issues
- **Trade-off**: Slower tests, but higher confidence

---

## 🔒 US-18: Role-Based Access Control (RBAC) - Manager & Operator Roles

**User Story:**  
As a system administrator, I want to assign roles to users (Manager and Operator), so that Managers can view all tasks in the system while Operators only see their own tasks.

**GitHub Issue**: [#34](https://github.com/dkennyq/bla-task-management-system/issues/34)

### Acceptance Criteria
1. ✅ Database has `role` column in `users` table with CHECK constraint
2. ✅ New users default to "Operator" role when role not specified
3. ✅ JWT token includes "role" claim
4. ✅ Manager users can call GET /api/tasks and see ALL tasks from all users
5. ✅ Operator users can call GET /api/tasks and see ONLY their own tasks
6. ✅ Registration endpoint accepts optional `role` parameter
7. ✅ GET /api/users/me returns user's role
8. ✅ Invalid role values are rejected with 400 Bad Request

### Technical Details

#### API: PostgreSQL Database Migration
**Migration File**: `infrastructure/database/postgres/04-migration-add-user-roles.sql`

```sql
ALTER TABLE users 
ADD COLUMN role VARCHAR(50) NOT NULL DEFAULT 'Operator';

ALTER TABLE users 
ADD CONSTRAINT chk_user_role 
CHECK (role IN ('Manager', 'Operator'));

CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);
```

#### API: Users API - Update Response
**Endpoint**: `POST /api/users/register`

**Request (Updated):**
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "fullName": "string",
  "role": "Manager" | "Operator"  // OPTIONAL, defaults to "Operator"
}
```

**Response (Updated):**
```json
{
  "id": "guid",
  "username": "string",
  "email": "string",
  "fullName": "string",
  "role": "Operator"  // NEW
}
```

#### API: Tasks API - Query Logic Update
**Endpoint**: `GET /api/tasks`

**Behavior:**
- Extract `userId` and `role` from JWT token
- **If role = "Manager"**: Return ALL tasks in the system (no userId filter)
- **If role = "Operator"**: Return only tasks where `userId` matches

**Response (unchanged):**
```json
[
  {
    "id": "guid",
    "title": "string",
    "description": "string",
    "status": "Pending" | "InProgress" | "Completed",
    "priority": "Low" | "Medium" | "High",
    "dueDate": "ISO 8601 date",
    "userId": "guid",  // Shows which user created the task
    "createdAt": "ISO 8601 datetime",
    "updatedAt": "ISO 8601 datetime"
  }
]
```

### Implementation Guide (TDD)

#### Phase 1: Database & Domain Layer (Users API)
1. **Database Migration** (Infrastructure)
   - Create `infrastructure/database/postgres/04-migration-add-user-roles.sql`
   - Add `role` column with DEFAULT 'Operator'
   - Add CHECK constraint for valid roles
   - Update seed data: admin user = 'Manager'

2. **Domain Layer** (Users API)
   - Create `UsersApi.Domain/Enums/UserRole.cs` enum
   - Update `UserEntity.cs`: Add `UserRole Role` property
   - Write tests FIRST:
     - `Constructor_ShouldSetRoleToOperator_WhenNotProvided()`
     - `Constructor_ShouldSetRoleToManager_WhenProvided()`
   - Run tests (RED)
   - Update constructor to accept `UserRole role` parameter
   - Run tests (GREEN)

#### Phase 2: Application Layer (Users API)
1. **Commands**
   - Update `RegisterUserCommand`: Add `UserRole Role` property
   - Update `RegisterUserCommandHandler`: Pass role to entity
   - Write tests FIRST:
     - `Handle_ShouldRegisterOperatorUser_WhenRoleNotProvided()`
     - `Handle_ShouldRegisterManagerUser_WhenRoleIsManager()`

2. **Queries**
   - Update `UserDto`: Add `string Role` property
   - Update `GetUserByIdQueryHandler`: Map `user.Role` to DTO

#### Phase 3: Infrastructure Layer (Users API)
1. **Repository**
   - Update `UserRepository.MapReader()`: Read `role` from DB, convert to enum
   - Update `UserRepository.AddAsync()`: Insert `role` column
   - Write tests FIRST:
     - `AddAsync_ShouldPersistUserWithRole()`
     - `GetByIdAsync_ShouldReturnUserWithCorrectRole()`

2. **JWT Service**
   - Update `JwtService.GenerateToken()`: Add role claim
   ```csharp
   new Claim("role", user.Role.ToString())
   ```
   - Write test: `GenerateToken_ShouldIncludeRoleClaim()`

#### Phase 4: WebApi Layer (Users API)
1. **DTOs**
   - Update `RegisterRequest`: Add `string? Role` property (optional)
   - Update `UserResponse`: Add `string Role` property

2. **Controller**
   - Update `UsersController.Register()`:
     - Validate role value (must be "Manager" or "Operator")
     - Default to "Operator" if not provided
   - Update `UsersController.GetMe()`: Include role in response
   - Write tests:
     - `Register_ShouldReturnBadRequest_WhenInvalidRole()`
     - `Register_ShouldCreateOperator_WhenRoleNotProvided()`

#### Phase 5: Tasks API - Query Logic
1. **Application Layer**
   - Update `GetAllTasksQuery`: Add `string UserRole` property
   - Update `GetAllTasksQueryHandler.Handle()`:
     ```csharp
     if (query.UserRole == "Manager")
         return await _repository.GetAllAsync(cancellationToken);
     else
         return await _repository.GetAllByUserIdAsync(query.UserId, cancellationToken);
     ```
   - Write tests:
     - `Handle_ShouldReturnAllTasks_WhenUserIsManager()`
     - `Handle_ShouldReturnOnlyUserTasks_WhenUserIsOperator()`

2. **Infrastructure Layer**
   - Add `ITaskRepository.GetAllAsync()` method
   - Implement in `MongoTaskRepository`:
     ```csharp
     public async Task<IEnumerable<TaskEntity>> GetAllAsync(CancellationToken cancellationToken)
     {
         return await _collection.Find(_ => true).ToListAsync(cancellationToken);
     }
     ```

3. **WebApi Layer**
   - Update `TasksController.GetAllTasks()`:
     - Extract role from JWT: `GetUserRole()` helper method
     - Pass role to query
   - Write tests: `GetAllTasks_ShouldCallHandlerWithManagerRole_WhenUserIsManager()`

### Files to Create/Modify

**Create New (2):**
1. `infrastructure/database/postgres/04-migration-add-user-roles.sql`
2. `apps/users-api/src/UsersApi.Domain/Enums/UserRole.cs`

**Modify Existing (24):**
- Users API: 16 files (Domain, Application, Infrastructure, WebApi layers + tests)
- Tasks API: 8 files (Application, Infrastructure, WebApi layers + tests)

### Dependencies
- ✅ Users API with registration (Issue #5)
- ✅ JWT authentication (Issue #19)
- ✅ Tasks API baseline
- 🔲 **Blocks**: Issue #35 (Real-Time Updates)

---

## 🔄 US-19: Real-Time Task Updates with SignalR

**User Story:**  
As a user, I want to see task changes in real-time when other users create, update, or delete tasks, so that I can collaborate effectively with my team without manually refreshing the page.

**GitHub Issue**: [#35](https://github.com/dkennyq/bla-task-management-system/issues/35)

### Acceptance Criteria
1. ✅ SignalR Hub is accessible at `/taskHub` endpoint
2. ✅ Frontend connects to SignalR on login
3. ✅ Frontend disconnects from SignalR on logout
4. ✅ TaskCreated event broadcasts when task is created via API
5. ✅ TaskUpdated event broadcasts when task is updated via API
6. ✅ TaskDeleted event broadcasts when task is deleted via API
7. ✅ Frontend updates task list reactively without page refresh
8. ✅ No duplicate tasks appear in UI
9. ✅ Connection reconnects automatically after interruption

### Technical Details

#### Backend: SignalR Hub Setup

**Hub Class**: `apps/tasks-api/src/TasksApi.WebApi/Hubs/TaskHub.cs`
```csharp
using Microsoft.AspNetCore.SignalR;

namespace TasksApi.WebApi.Hubs;

public class TaskHub : Hub
{
    public async Task JoinUserRoom(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}
```

**Program.cs Registration:**
```csharp
// Add services
builder.Services.AddSignalR();

// Map hub endpoint
app.MapHub<TaskHub>("/taskHub");
```

**CORS Update:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // REQUIRED for SignalR
    });
});
```

#### Backend: Broadcast Events from TasksController

**Inject IHubContext:**
```csharp
private readonly IHubContext<TaskHub> _hubContext;

public TasksController(IHubContext<TaskHub> hubContext, /* other deps */)
{
    _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
}
```

**Create Task:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
{
    // ... existing logic ...
    var task = await _commandHandler.HandleAsync(command, cancellationToken);
    
    // Broadcast to all connected clients
    await _hubContext.Clients.All.SendAsync("TaskCreated", task, cancellationToken);
    
    return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
}
```

**Update Task:**
```csharp
await _hubContext.Clients.All.SendAsync("TaskUpdated", updatedTask, cancellationToken);
```

**Delete Task:**
```csharp
await _hubContext.Clients.All.SendAsync("TaskDeleted", new { id, userId = GetUserId() }, cancellationToken);
```

#### Frontend: SignalR Service

**Install Package:**
```bash
npm install @microsoft/signalr
```

**Service File**: `apps/frontend/src/services/signalrService.js`
```javascript
import * as signalR from '@microsoft/signalr';
import { useTasksStore } from '@/stores/tasksStore';

let connection = null;

export async function connectSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5077/taskHub')
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .build();

    const tasksStore = useTasksStore();

    // Event handlers
    connection.on('TaskCreated', (task) => {
        tasksStore.addTaskFromSignalR(task);
    });

    connection.on('TaskUpdated', (task) => {
        tasksStore.updateTaskFromSignalR(task);
    });

    connection.on('TaskDeleted', ({ id }) => {
        tasksStore.removeTaskFromSignalR(id);
    });

    await connection.start();
    console.log('✅ SignalR connected');
}

export async function disconnectSignalR() {
    if (connection) {
        await connection.stop();
    }
}
```

#### Frontend: Pinia Store Updates

**File**: `apps/frontend/src/stores/tasksStore.js`
```javascript
export const useTasksStore = defineStore('tasks', {
    state: () => ({
        tasks: []
    }),
    
    actions: {
        addTaskFromSignalR(task) {
            const exists = this.tasks.find(t => t.id === task.id);
            if (!exists) {
                this.tasks.unshift(task);
            }
        },

        updateTaskFromSignalR(task) {
            const index = this.tasks.findIndex(t => t.id === task.id);
            if (index !== -1) {
                this.tasks[index] = task;
            } else {
                this.tasks.unshift(task);
            }
        },

        removeTaskFromSignalR(taskId) {
            const index = this.tasks.findIndex(t => t.id === taskId);
            if (index !== -1) {
                this.tasks.splice(index, 1);
            }
        }
    }
});
```

#### Frontend: App.vue Lifecycle

**Connect on mount:**
```vue
<script setup>
import { onMounted, onUnmounted } from 'vue';
import { connectSignalR, disconnectSignalR } from '@/services/signalrService';
import { useAuthStore } from '@/stores/authStore';

const authStore = useAuthStore();

onMounted(async () => {
    if (authStore.isAuthenticated) {
        await connectSignalR();
    }
});

onUnmounted(async () => {
    await disconnectSignalR();
});
</script>
```

### Implementation Guide (TDD)

#### Phase 1: Backend Setup
1. **Create TaskHub** (WebApi Layer)
   - Create `TasksApi.WebApi/Hubs/TaskHub.cs`
   - Register in `Program.cs`
   - Update CORS to allow credentials

2. **Inject IHubContext into TasksController**
   - Add constructor parameter
   - Write mock tests:
     - `CreateTask_ShouldBroadcastTaskCreatedEvent_WhenSuccessful()`
     - `UpdateTask_ShouldBroadcastTaskUpdatedEvent_WhenSuccessful()`
     - `DeleteTask_ShouldBroadcastTaskDeletedEvent_WhenSuccessful()`
   - Run tests (RED)
   - Add broadcast calls after successful operations
   - Run tests (GREEN)

#### Phase 2: Frontend Setup
1. **Install SignalR Client**
   - Run `npm install @microsoft/signalr`

2. **Create SignalR Service**
   - Create `src/services/signalrService.js`
   - Implement `connectSignalR()` and `disconnectSignalR()`
   - Add event handlers for TaskCreated, TaskUpdated, TaskDeleted

3. **Update Pinia Store**
   - Add `addTaskFromSignalR()` action (avoid duplicates)
   - Add `updateTaskFromSignalR()` action
   - Add `removeTaskFromSignalR()` action

4. **Connect on Login/Mount**
   - Update `App.vue`: Connect on mount if authenticated
   - Update `LoginView.vue`: Connect after successful login
   - Update `authStore.js`: Disconnect on logout

#### Phase 3: Testing
1. **Backend Tests**
   - Mock `IHubContext<TaskHub>`
   - Verify `SendAsync()` called with correct event name and payload
   - Run `dotnet test`

2. **Frontend Manual Tests**
   - Open two browser windows (User A and User B)
   - User A creates task → User B sees it appear instantly
   - User B updates task → User A sees update instantly
   - User A deletes task → User B sees it disappear instantly
   - Test reconnection: Stop API → Verify "Reconnecting..." → Restart API → Verify reconnects

### Files to Create/Modify

**Create New (3):**
1. `apps/tasks-api/src/TasksApi.WebApi/Hubs/TaskHub.cs`
2. `apps/frontend/src/services/signalrService.js`
3. `apps/frontend/src/components/SignalRStatus.vue` (optional indicator)

**Modify Existing (8):**
- Backend: 3 files (`Program.cs`, `TasksController.cs`, `TasksControllerTests.cs`)
- Frontend: 5 files (`tasksStore.js`, `App.vue`, `LoginView.vue`, `authStore.js`, `package.json`)

### Dependencies
- ✅ Tasks API with CRUD (Issue #2, #4, #3)
- ✅ Frontend task list (Issue #10)
- ✅ Authentication (Issue #19)
- 🔲 **Recommended**: Issue #34 (RBAC - makes testing more interesting)

---

**Document Version**: 1.0  
**Last Updated**: 2026-06-10  
**Status**: Living Document (will be updated as features are completed)
