using FluentAssertions;
using Moq;
using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Application.Tests.Queries;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly GetTaskByIdQueryHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetTaskByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidIdAndOwner_ReturnsTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expectedTask = new TaskEntity(
            taskId,
            "Test Task",
            "Test Description",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1),
            userId
        );

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        var query = new GetTaskByIdQuery(taskId, userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedTask);
        _repositoryMock.Verify(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyId_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(Guid.Empty, userId);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Id*");
    }

    [Fact]
    public async Task Handle_EmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(taskId, Guid.Empty);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*UserId*");
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity?)null);

        var query = new GetTaskByIdQuery(taskId, userId);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with id {taskId} not found");
    }

    [Fact]
    public async Task Handle_UserNotOwner_ThrowsForbiddenException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        
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

        var query = new GetTaskByIdQuery(taskId, differentUserId);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("User does not have permission to access this task");
    }
}
