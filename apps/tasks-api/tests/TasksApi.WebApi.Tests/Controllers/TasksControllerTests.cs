using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;
using TasksApi.WebApi.Controllers;
using Xunit;

namespace TasksApi.WebApi.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TasksController _controller;
    private readonly GetAllTasksQueryHandler _handler;

    public TasksControllerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetAllTasksQueryHandler(_repositoryMock.Object);
        _controller = new TasksController(_handler);
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOkResult_WhenTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<TaskEntity>
        {
            new TaskEntity(
                Guid.NewGuid(),
                "Task 1",
                "Description 1",
                TaskEntityStatus.Pending,
                TaskPriority.High,
                DateTime.UtcNow.AddDays(1),
                userId
            ),
            new TaskEntity(
                Guid.NewGuid(),
                "Task 2",
                "Description 2",
                TaskEntityStatus.InProgress,
                TaskPriority.Medium,
                DateTime.UtcNow.AddDays(2),
                userId
            )
        };

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _controller.GetAllTasks(userId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOkResult_WithEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyList = new List<TaskEntity>();

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllTasks(userId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tasks = okResult!.Value as IEnumerable<TaskEntity>;
        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllTasks_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        // Arrange
        var emptyUserId = Guid.Empty;

        // Act
        var result = await _controller.GetAllTasks(emptyUserId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
