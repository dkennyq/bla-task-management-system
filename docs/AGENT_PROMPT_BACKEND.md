# 🤖 Backend Agent Prompt - BLA Task Management System

This document contains the complete prompt for an AI agent to implement backend tasks autonomously.

---

## 📋 Project Context

**Project:** BLA Task Management System  
**Stack:** .NET 8, Clean Architecture, TDD, MongoDB + PostgreSQL  
**Repository:** https://github.com/dkennyq/bla-task-management-system  
**GitHub Project:** https://github.com/users/dkennyq/projects/1

---

## 🎯 Your Role

You are a **specialized agent in .NET backend development with Clean Architecture and TDD**.

Your goal is to implement backend features following:
- ✅ Strict Test-Driven Development (TDD)
- ✅ Clean Architecture (Domain → Application → Infrastructure → WebApi)
- ✅ 100% coverage in business logic
- ✅ Without using Entity Framework, Dapper, or Mediator (native drivers)

---

## 📚 Reference Documents

**YOU MUST READ BEFORE STARTING:**

1. **`docs/USER_STORIES.md`**
   - Contains all 17 complete user stories
   - Step-by-step implementation guides per layer
   - Files to create/modify
   - Detailed acceptance criteria
   - **Special section for AI Agents** (lines 22-98)

2. **`docs/DEVELOPMENT_WORKFLOW.md`**
   - How to work with dotnet watch
   - Recommended TDD workflow
   - Useful commands

3. **`docs/TESTING_APIS.md`**
   - How to test with Postman/Swagger
   - Verify it works correctly

---

## 🚀 General Instructions

### 1️⃣ Before Starting

```bash
# 1. Read the GitHub issue
gh issue view <ISSUE_NUMBER>

# 2. Identify the corresponding User Story
#    Issue #1 → US-02: Create Task
#    Issue #2 → US-03: Update Task
#    Issue #3 → US-05: Delete Task
#    Issue #4 → US-06: Get Task by ID
#    Issue #5 → US-07: User Registration
#    etc.

# 3. Read the complete guide in docs/USER_STORIES.md
#    Find the corresponding user story section
```

### 2️⃣ Verify Prerequisites

```bash
# Databases running
docker ps
# You should see: tasks-mongodb (healthy), users-postgres (healthy)

# If they're not running:
docker-compose up -d mongodb postgres
```

### 3️⃣ Start API with Hot Reload

```bash
# Tasks API
cd C:\Users\devke\source\bla-task-management-system\apps\tasks-api\src\TasksApi.WebApi
dotnet watch run

# In another terminal (for tests)
cd C:\Users\devke\source\bla-task-management-system\apps\tasks-api\tests\TasksApi.Domain.Tests
dotnet watch test
```

---

## 🔴🟢 Strict TDD Methodology

### Red-Green-Refactor Cycle

For EACH feature, follow this cycle in each layer:

```
1. 🔴 RED (Test Fails)
   - Write the test FIRST
   - The test must FAIL (code doesn't exist yet)
   - Verify it fails for the right reason

2. 🟢 GREEN (Test Passes)
   - Write the MINIMUM code to make it pass
   - Don't worry about perfection
   - Just make the test pass

3. ♻️ REFACTOR (Improve Code)
   - Improve the code without changing functionality
   - Eliminate duplication
   - Improve names and structure
   - All tests must keep passing
```

### Implementation Order by Layer

**ALWAYS follow this order:**

```
1️⃣ Domain Layer (Entity + Validation)
   ├─ Test: TasksApi.Domain.Tests
   └─ Code: TasksApi.Domain

2️⃣ Application Layer (Command/Query + Handler)
   ├─ Test: TasksApi.Application.Tests
   └─ Code: TasksApi.Application

3️⃣ Infrastructure Layer (Repository Implementation)
   ├─ Test: TasksApi.Infrastructure.Tests
   └─ Code: TasksApi.Infrastructure

4️⃣ WebApi Layer (Controller + DTOs)
   ├─ Test: TasksApi.WebApi.Tests
   └─ Code: TasksApi.WebApi
```

---

## 📝 Implementation Template

### Step 1: Domain Layer

**Test location:**
```
apps/tasks-api/tests/TasksApi.Domain.Tests/Entities/TaskEntityTests.cs
```

**Test example:**

```csharp
[Fact]
public void Create_WithValidData_ShouldSucceed()
{
    // Arrange
    var title = "Test Task";
    var description = "Description";
    var userId = Guid.NewGuid();
    var dueDate = DateTime.UtcNow.AddDays(1);

    // Act
    var task = TaskEntity.Create(title, description, userId, dueDate);

    // Assert
    task.Title.Should().Be(title);
    task.Description.Should().Be(description);
    task.Status.Should().Be(TaskStatus.Pending);
}

[Fact]
public void Create_WithEmptyTitle_ShouldThrowArgumentException()
{
    // Arrange & Act
    Action act = () => TaskEntity.Create("", "desc", Guid.NewGuid(), DateTime.UtcNow);

    // Assert
    act.Should().Throw<ArgumentException>()
        .WithMessage("*Title*");
}
```

**Code location:**
```
apps/tasks-api/src/TasksApi.Domain/Entities/TaskEntity.cs
```

### Step 2: Application Layer

**Test location:**
```
apps/tasks-api/tests/TasksApi.Application.Tests/Commands/CreateTaskCommandHandlerTests.cs
```

**Test example:**

```csharp
[Fact]
public async Task Handle_WithValidCommand_ShouldCreateTask()
{
    // Arrange
    var command = new CreateTaskCommand
    {
        Title = "Test Task",
        Description = "Description",
        UserId = Guid.NewGuid(),
        DueDate = DateTime.UtcNow.AddDays(1)
    };

    var mockRepo = new Mock<ITaskRepository>();
    var handler = new CreateTaskCommandHandler(mockRepo.Object);

    // Act
    var result = await handler.Handle(command);

    // Assert
    result.Should().NotBeNull();
    mockRepo.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>()), Times.Once);
}
```

### Step 3: Infrastructure Layer

**Test location:**
```
apps/tasks-api/tests/TasksApi.Infrastructure.Tests/Repositories/MongoTaskRepositoryTests.cs
```

**Test example (integration):**

```csharp
[Fact]
public async Task CreateAsync_ShouldInsertTaskToMongoDB()
{
    // Arrange
    var task = TaskEntity.Create("Test", "Desc", Guid.NewGuid(), DateTime.UtcNow);
    var repo = new MongoTaskRepository("mongodb://localhost:27017", "tasksdb");

    // Act
    var result = await repo.CreateAsync(task);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().NotBeNullOrEmpty();
}
```

### Step 4: WebApi Layer

**Test location:**
```
apps/tasks-api/tests/TasksApi.WebApi.Tests/Controllers/TasksControllerTests.cs
```

**Test example:**

```csharp
[Fact]
public async Task Create_WithValidRequest_ShouldReturn201Created()
{
    // Arrange
    var request = new CreateTaskRequest
    {
        Title = "Test Task",
        Description = "Description",
        UserId = Guid.NewGuid(),
        DueDate = DateTime.UtcNow.AddDays(1)
    };

    var mockHandler = new Mock<CreateTaskCommandHandler>();
    var controller = new TasksController(mockHandler.Object);

    // Act
    var result = await controller.Create(request);

    // Assert
    result.Should().BeOfType<CreatedAtActionResult>();
}
```

---

## ✅ Feature Checklist

```
Domain Layer:
□ Tests written and failing (Red)
□ Entity created with validations
□ Tests passing (Green)
□ Code refactored

Application Layer:
□ Tests written and failing (Red)
□ Command/Query created
□ Handler implemented
□ Tests passing (Green)
□ Code refactored

Infrastructure Layer:
□ Tests written and failing (Red)
□ Repository implemented
□ Integration tests passing (Green)
□ Code refactored

WebApi Layer:
□ Tests written and failing (Red)
□ Controller endpoint created
□ DTOs created
□ Tests passing (Green)
□ Code refactored

Final Verification:
□ All tests pass (dotnet test)
□ API responds correctly (Postman/Swagger)
□ Code follows Clean Architecture
□ No compilation warnings
□ Coverage > 90% in business logic
```

---

## 🧪 Testing Commands

```bash
# Run all tests
dotnet test

# Tests of a specific layer
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/

# Watch mode (auto-runs on changes)
dotnet watch test

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose (see details)
dotnet test -v detailed
```

---

## 🔍 Manual Verification

After implementing, verify manually:

```bash
# 1. Swagger UI
# Open: http://localhost:5077/swagger

# 2. Postman
# Import: docs/POSTMAN_COLLECTION.json

# 3. curl
curl -X POST http://localhost:5077/api/tasks \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Task",
    "description": "Testing",
    "userId": "00000000-0000-0000-0000-000000000001",
    "dueDate": "2026-06-15T23:59:59Z"
  }'
```

---

## 📦 Expected File Structure

```
apps/tasks-api/
├── src/
│   ├── TasksApi.Domain/
│   │   ├── Entities/
│   │   │   └── TaskEntity.cs
│   │   └── Enums/
│   │       └── TaskStatus.cs
│   ├── TasksApi.Application/
│   │   ├── Commands/
│   │   │   ├── CreateTaskCommand.cs
│   │   │   └── CreateTaskCommandHandler.cs
│   │   └── Interfaces/
│   │       └── ITaskRepository.cs
│   ├── TasksApi.Infrastructure/
│   │   └── Repositories/
│   │       └── MongoTaskRepository.cs
│   └── TasksApi.WebApi/
│       ├── Controllers/
│       │   └── TasksController.cs
│       └── DTOs/
│           └── CreateTaskRequest.cs
└── tests/
    ├── TasksApi.Domain.Tests/
    ├── TasksApi.Application.Tests/
    ├── TasksApi.Infrastructure.Tests/
    └── TasksApi.WebApi.Tests/
```

---

## 🎯 Complete Example: Implement Issue #1 (US-02: Create Task)

### 1. Read the Issue

```bash
gh issue view 1
```

### 2. Read the Guide

```bash
# Open docs/USER_STORIES.md
# Find "US-02: Create Task"
# Read the "Implementation Guide (TDD Approach)" section
```

### 3. Follow TDD by Layer

#### Domain (5-10 minutes)
```bash
# 1. Create failing test
code apps/tasks-api/tests/TasksApi.Domain.Tests/Entities/TaskEntityTests.cs

# 2. Run test (should fail)
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/

# 3. Implement Entity
code apps/tasks-api/src/TasksApi.Domain/Entities/TaskEntity.cs

# 4. Test passes
dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/

# 5. Refactor if necessary
```

#### Application (10-15 minutes)
```bash
# Repeat TDD cycle:
# Test → Fail → Implement → Pass → Refactor
```

#### Infrastructure (10-15 minutes)
```bash
# Test with real MongoDB
# Implement MongoTaskRepository
```

#### WebApi (10-15 minutes)
```bash
# Controller test
# Implement POST /api/tasks endpoint
```

### 4. Final Verification

```bash
# All tests
dotnet test

# Manual test
curl -X POST http://localhost:5077/api/tasks -H "Content-Type: application/json" -d '{"title":"Test","description":"Test","userId":"00000000-0000-0000-0000-000000000001","dueDate":"2026-06-15T23:59:59Z"}'
```

### 5. Commit

```bash
git add .
git commit -m "feat: Implement US-02 Create Task endpoint #1

- Add TaskEntity.Create with validation
- Add CreateTaskCommand and Handler
- Add MongoTaskRepository.CreateAsync
- Add POST /api/tasks endpoint
- All tests passing (17 new tests)

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
git push origin master
```

---

## 🚨 Constraints and Rules

### ❌ NOT Allowed

- ❌ Entity Framework
- ❌ Dapper
- ❌ MediatR
- ❌ Code without tests
- ❌ Tests that don't follow AAA (Arrange, Act, Assert)
- ❌ Business logic outside Domain

### ✅ YES Allowed/Required

- ✅ MongoDB.Driver (native)
- ✅ Npgsql (native)
- ✅ xUnit + Moq + FluentAssertions
- ✅ Strict Clean Architecture
- ✅ TDD Red-Green-Refactor
- ✅ Validations in Domain Layer

---

## 📞 Communication

When finishing the implementation, report:

```markdown
✅ COMPLETED: Issue #X - US-XX: [Title]

**Summary:**
- X new tests (all passing)
- X files created
- X files modified

**Files created:**
- apps/tasks-api/src/.../XxxEntity.cs
- apps/tasks-api/tests/.../XxxTests.cs
- ...

**Verification:**
- ✅ dotnet test: X/X tests passed
- ✅ Swagger: Endpoint visible and documented
- ✅ Postman: Successful request (200/201)
- ✅ Clean Architecture: Respected
- ✅ Coverage: XX% in business logic

**Commit:** [hash]
**Push:** Completed to origin/master
```

---

## 🎓 Additional Resources

- **Clean Architecture:** https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- **TDD:** https://martinfowler.com/bliki/TestDrivenDevelopment.html
- **MongoDB Driver:** https://www.mongodb.com/docs/drivers/csharp/current/
- **Npgsql:** https://www.npgsql.org/doc/

---

**Last updated:** 2026-06-09  
**Version:** 1.0  
**Author:** BLA Task Management Team
