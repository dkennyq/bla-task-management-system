using FluentAssertions;
using Moq;
using TasksApi.Application.Commands;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Application.Tests.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new CreateTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(7),
            UserId = Guid.NewGuid()
        };

        TaskEntity? capturedTask = null;
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) =>
            {
                capturedTask = task;
                return task;
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().Be(command.Description);
        result.Priority.Should().Be(command.Priority);
        result.DueDate.Should().BeCloseTo(command.DueDate.Value, TimeSpan.FromSeconds(1));
        result.UserId.Should().Be(command.UserId);
        result.Status.Should().Be(TaskEntityStatus.Pending);
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        
        capturedTask.Should().NotBeNull();
        capturedTask!.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task Handle_WithoutDescription_ShouldCreateTask()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = null,
            Priority = TaskPriority.Medium,
            DueDate = null,
            UserId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.Description.Should().BeNull();
        result.Status.Should().Be(TaskEntityStatus.Pending);
        
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "",
            Description = "Description",
            Priority = TaskPriority.Low,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*title*");
        
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Description",
            Priority = TaskPriority.Low,
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.Empty
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*userId*");
        
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetStatusToPending()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Description",
            Priority = TaskPriority.Medium,
            DueDate = DateTime.UtcNow.AddDays(5),
            UserId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.Should().Be(TaskEntityStatus.Pending);
    }

    [Fact]
    public async Task Handle_ShouldGenerateNewId()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Description",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(3),
            UserId = Guid.NewGuid()
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskEntity task, CancellationToken ct) => task);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Id.Should().NotBe(Guid.Empty);
    }
}
