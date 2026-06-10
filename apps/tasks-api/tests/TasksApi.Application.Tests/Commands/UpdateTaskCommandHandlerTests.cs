using FluentAssertions;
using Moq;
using TasksApi.Application.Commands;
using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Application.Tests.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new UpdateTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTask_WhenValidCommandProvided()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Original Title",
            description: "Original Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10),
            UserId = userId
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(taskId);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(TaskPriority.High);
        result.Status.Should().Be(TaskEntityStatus.InProgress);
        result.DueDate.Should().BeCloseTo(command.DueDate.Value, TimeSpan.FromSeconds(1));
        result.UserId.Should().Be(userId);
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10),
            UserId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with ID {taskId} not found");

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserIsNotOwner()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Original Title",
            description: "Original Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: ownerId
        );

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10),
            UserId = differentUserId // Different user trying to update
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"User {differentUserId} is not authorized to update task {taskId}");

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllFields_WhenAllFieldsProvided()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Original Title",
            description: "Original Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var newDueDate = DateTime.UtcNow.AddDays(15);
        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Completely New Title",
            Description = "Completely New Description",
            Priority = TaskPriority.Urgent,
            Status = TaskEntityStatus.Completed,
            DueDate = newDueDate,
            UserId = userId
        };

        TaskEntity? capturedTask = null;
        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) =>
            {
                capturedTask = task;
                return task;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTask.Should().NotBeNull();
        capturedTask!.Title.Should().Be("Completely New Title");
        capturedTask.Description.Should().Be("Completely New Description");
        capturedTask.Priority.Should().Be(TaskPriority.Urgent);
        capturedTask.Status.Should().Be(TaskEntityStatus.Completed);
        capturedTask.DueDate.Should().BeCloseTo(newDueDate, TimeSpan.FromSeconds(1));
        capturedTask.UserId.Should().Be(userId); // Should remain unchanged
        capturedTask.Id.Should().Be(taskId); // Should remain unchanged
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Original Title",
            description: "Original Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "Updated Title",
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10),
            UserId = userId
        };

        var callOrder = new List<string>();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask)
            .Callback(() => callOrder.Add("GetByIdAsync"));

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) => task)
            .Callback(() => callOrder.Add("UpdateAsync"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be("GetByIdAsync");
        callOrder[1].Should().Be("UpdateAsync");
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentException_WhenTitleIsEmpty()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Original Title",
            description: "Original Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var command = new UpdateTaskCommand
        {
            Id = taskId,
            Title = "", // Empty title
            Description = "Updated Description",
            Priority = TaskPriority.High,
            Status = TaskEntityStatus.InProgress,
            DueDate = DateTime.UtcNow.AddDays(10),
            UserId = userId
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*title*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
