using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TasksApi.Application.Commands;
using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;
using TasksApi.WebApi.Controllers;
using TasksApi.WebApi.DTOs;
using Xunit;

namespace TasksApi.WebApi.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ICreateTaskCommandHandler> _createHandlerMock;
    private readonly Mock<IUpdateTaskCommandHandler> _updateHandlerMock;
    private readonly Mock<IDeleteTaskCommandHandler> _deleteHandlerMock;
    private readonly TasksController _controller;
    private readonly GetAllTasksQueryHandler _queryHandler;
    private readonly GetTaskByIdQueryHandler _getByIdQueryHandler;
    private readonly Guid _testUserId;

    public TasksControllerTests()
    {
        _testUserId = Guid.NewGuid();
        _repositoryMock = new Mock<ITaskRepository>();
        _queryHandler = new GetAllTasksQueryHandler(_repositoryMock.Object);
        _getByIdQueryHandler = new GetTaskByIdQueryHandler(_repositoryMock.Object);
        _createHandlerMock = new Mock<ICreateTaskCommandHandler>();
        _updateHandlerMock = new Mock<IUpdateTaskCommandHandler>();
        _deleteHandlerMock = new Mock<IDeleteTaskCommandHandler>();
        _controller = new TasksController(
            _queryHandler, 
            _getByIdQueryHandler, 
            _createHandlerMock.Object, 
            _updateHandlerMock.Object, 
            _deleteHandlerMock.Object);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOkResult_WhenTasksExist()
    {
        // Arrange
        var tasks = new List<TaskEntity>
        {
            new TaskEntity(
                Guid.NewGuid(),
                "Task 1",
                "Description 1",
                TaskEntityStatus.Pending,
                TaskPriority.High,
                DateTime.UtcNow.AddDays(1),
                _testUserId
            ),
            new TaskEntity(
                Guid.NewGuid(),
                "Task 2",
                "Description 2",
                TaskEntityStatus.InProgress,
                TaskPriority.Medium,
                DateTime.UtcNow.AddDays(2),
                _testUserId
            )
        };

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAllTasks();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOkResult_WithEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var emptyList = new List<TaskEntity>();

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(_testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllTasks();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tasks = okResult!.Value as IEnumerable<TaskEntity>;
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "New Task",
            Description = "Task Description",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createdTask = new TaskEntity(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            TaskEntityStatus.Pending,
            request.Priority,
            request.DueDate,
            _testUserId
        );

        _createHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        createdAtActionResult!.ActionName.Should().Be("GetTaskById");
        createdAtActionResult.RouteValues!["id"].Should().Be(createdTask.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(createdTask);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "",
            Description = "Description",
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _createHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Title cannot be empty", "title"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldUseUserIdFromClaims()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Task with Claim UserId",
            Description = "Should use userId from JWT claims",
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        var createdTask = new TaskEntity(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            TaskEntityStatus.Pending,
            request.Priority,
            request.DueDate,
            _testUserId
        );

        CreateTaskCommand? capturedCommand = null;
        _createHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .Callback<CreateTaskCommand, CancellationToken>((cmd, ct) => capturedCommand = cmd)
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        capturedCommand.Should().NotBeNull();
        capturedCommand!.UserId.Should().Be(_testUserId);
    }

    [Fact]
    public async Task Create_ShouldSetStatusToPending()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Task with Default Status",
            Description = "Should default to Pending",
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        TaskEntity? capturedTask = null;
        _createHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateTaskCommand cmd, CancellationToken ct) =>
            {
                capturedTask = new TaskEntity(
                    Guid.NewGuid(),
                    cmd.Title,
                    cmd.Description,
                    TaskEntityStatus.Pending,
                    cmd.Priority,
                    cmd.DueDate,
                    cmd.UserId
                );
                return capturedTask;
            });

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        capturedTask.Should().NotBeNull();
        capturedTask!.Status.Should().Be(TaskEntityStatus.Pending);
    }

    [Fact]
    public async Task Create_WithoutOptionalFields_ShouldSucceed()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Minimal Task",
            Description = null,
            Priority = TaskPriority.Low,
            DueDate = null
        };

        var createdTask = new TaskEntity(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            TaskEntityStatus.Pending,
            request.Priority,
            request.DueDate,
            _testUserId
        );

        _createHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = result.Result as CreatedAtActionResult;
        var returnedTask = createdAtActionResult!.Value as TaskEntity;
        returnedTask!.Description.Should().BeNull();
        returnedTask.DueDate.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTask_ShouldReturn200_WhenTaskUpdatedSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.Urgent,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(15)
        };

        var updatedTask = new TaskEntity(
            taskId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            _testUserId
        );

        _updateHandlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _controller.UpdateTask(taskId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(updatedTask);

        _updateHandlerMock.Verify(h => h.Handle(
            It.Is<UpdateTaskCommand>(c =>
                c.Id == taskId &&
                c.Title == request.Title &&
                c.Description == request.Description &&
                c.Priority == request.Priority &&
                c.Status == request.Status &&
                c.UserId == _testUserId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturn404_WhenTaskNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _updateHandlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Task with ID {taskId} not found"));

        // Act
        var result = await _controller.UpdateTask(taskId, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTask_ShouldReturn403_WhenUserIsNotOwner()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _updateHandlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException($"User {_testUserId} is not authorized to update task {taskId}"));

        // Act
        var result = await _controller.UpdateTask(taskId, request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(403);
        objectResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTask_ShouldReturn400_WhenInvalidRequest()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest
        {
            Title = "", // Invalid empty title
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        _updateHandlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Title cannot be empty", "title"));

        // Act
        var result = await _controller.UpdateTask(taskId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTask_ShouldCallHandler_WithCorrectCommand()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(20);
        var request = new UpdateTaskRequest
        {
            Title = "Specific Title",
            Description = "Specific Description",
            Priority = TaskPriority.Low,
            Status = TaskEntityStatus.Completed,
            DueDate = dueDate
        };

        var updatedTask = new TaskEntity(
            taskId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            _testUserId
        );

        UpdateTaskCommand? capturedCommand = null;
        _updateHandlerMock
            .Setup(h => h.Handle(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>()))
            .Callback<UpdateTaskCommand, CancellationToken>((cmd, ct) => capturedCommand = cmd)
            .ReturnsAsync(updatedTask);

        // Act
        await _controller.UpdateTask(taskId, request);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Id.Should().Be(taskId);
        capturedCommand.Title.Should().Be(request.Title);
        capturedCommand.Description.Should().Be(request.Description);
        capturedCommand.Priority.Should().Be(request.Priority);
        capturedCommand.Status.Should().Be(request.Status);
        capturedCommand.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
        capturedCommand.UserId.Should().Be(_testUserId);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturn204_WhenTaskDeletedSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _deleteHandlerMock
            .Setup(h => h.Handle(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        _deleteHandlerMock.Verify(h => h.Handle(
            It.Is<DeleteTaskCommand>(c =>
                c.Id == taskId &&
                c.UserId == _testUserId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturn404_WhenTaskNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _deleteHandlerMock
            .Setup(h => h.Handle(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Task with ID {taskId} not found"));

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTask_ShouldReturn403_WhenUserIsNotOwner()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _deleteHandlerMock
            .Setup(h => h.Handle(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenException($"User {_testUserId} is not authorized to delete task {taskId}"));

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(403);
        objectResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTask_ShouldCallHandler_WithCorrectCommand()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        DeleteTaskCommand? capturedCommand = null;
        _deleteHandlerMock
            .Setup(h => h.Handle(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>()))
            .Callback<DeleteTaskCommand, CancellationToken>((cmd, ct) => capturedCommand = cmd)
            .Returns(Task.CompletedTask);

        // Act
        await _controller.DeleteTask(taskId);

        // Assert
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Id.Should().Be(taskId);
        capturedCommand.UserId.Should().Be(_testUserId);
    }

    [Fact]
    public async Task GetTaskById_ValidRequest_Returns200WithTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedTask = new TaskEntity(
            taskId,
            "Test Task",
            "Test Description",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1),
            _testUserId
        );

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task GetTaskById_TaskNotFound_Returns404()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTaskById_UserNotOwner_Returns403()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        
        var task = new TaskEntity(
            taskId,
            "Test Task",
            "Test Description",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1),
            ownerId
        );

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetTaskById_EmptyId_Returns400()
    {
        // Act
        var result = await _controller.GetTaskById(Guid.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetAllTasks_Returns401_WhenUserNotAuthenticated()
    {
        // Arrange
        var unauthenticatedController = new TasksController(
            _queryHandler,
            _getByIdQueryHandler,
            _createHandlerMock.Object,
            _updateHandlerMock.Object,
            _deleteHandlerMock.Object);

        unauthenticatedController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await unauthenticatedController.GetAllTasks();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_Returns401_WhenUserNotAuthenticated()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "Unauthorized Task"
        };

        var unauthenticatedController = new TasksController(
            _queryHandler,
            _getByIdQueryHandler,
            _createHandlerMock.Object,
            _updateHandlerMock.Object,
            _deleteHandlerMock.Object);

        unauthenticatedController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await unauthenticatedController.Create(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
