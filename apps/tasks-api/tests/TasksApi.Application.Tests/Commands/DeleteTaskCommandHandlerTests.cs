using FluentAssertions;
using Moq;
using TasksApi.Application.Commands;
using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Application.Tests.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new DeleteTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteTask_WhenValidCommandProvided()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Task to Delete",
            description: "This task will be deleted",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var command = new DeleteTaskCommand
        {
            Id = taskId,
            UserId = userId
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(r => r.DeleteAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand
        {
            Id = taskId,
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
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
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
            title: "Someone else's task",
            description: "This task belongs to another user",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: ownerId
        );

        var command = new DeleteTaskCommand
        {
            Id = taskId,
            UserId = differentUserId // Different user trying to delete
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"User {differentUserId} is not authorized to delete task {taskId}");

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryMethods_InCorrectOrder()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTask = new TaskEntity(
            id: taskId,
            title: "Task to Delete",
            description: "This task will be deleted",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Low,
            dueDate: DateTime.UtcNow.AddDays(5),
            userId: userId
        );

        var command = new DeleteTaskCommand
        {
            Id = taskId,
            UserId = userId
        };

        var callOrder = new List<string>();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask)
            .Callback(() => callOrder.Add("GetByIdAsync"));

        _repositoryMock
            .Setup(r => r.DeleteAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Callback(() => callOrder.Add("DeleteAsync"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be("GetByIdAsync");
        callOrder[1].Should().Be("DeleteAsync");
    }
}
