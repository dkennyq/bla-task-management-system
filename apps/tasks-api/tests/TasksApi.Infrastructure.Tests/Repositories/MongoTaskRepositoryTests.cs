using FluentAssertions;
using MongoDB.Driver;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;
using TasksApi.Infrastructure.Repositories;
using Xunit;

namespace TasksApi.Infrastructure.Tests.Repositories;

public class MongoTaskRepositoryTests : IAsyncLifetime
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _database;
    private readonly ITaskRepository _repository;
    private readonly string _testCollectionName = "tasks_test";

    public MongoTaskRepositoryTests()
    {
        var connectionString = "mongodb://localhost:27017";
        _mongoClient = new MongoClient(connectionString);
        _database = _mongoClient.GetDatabase("tasksdb");
        _repository = new MongoTaskRepository(connectionString, "tasksdb", _testCollectionName);
    }

    public async Task InitializeAsync()
    {
        // Limpiar la colección antes de cada test
        await _database.DropCollectionAsync(_testCollectionName);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _repository.GetAllByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTask_AndReturnCreatedTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskEntity(
            Guid.NewGuid(),
            "Test Task",
            "Test Description",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(7),
            userId
        );

        // Act
        var result = await _repository.CreateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Title.Should().Be(task.Title);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnUserTasks_WhenTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task1 = new TaskEntity(
            Guid.NewGuid(),
            "Task 1",
            "Description 1",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1),
            userId
        );
        var task2 = new TaskEntity(
            Guid.NewGuid(),
            "Task 2",
            "Description 2",
            TaskEntityStatus.InProgress,
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(2),
            userId
        );

        await _repository.CreateAsync(task1);
        await _repository.CreateAsync(task2);

        // Act
        var result = await _repository.GetAllByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Id == task1.Id);
        result.Should().Contain(t => t.Id == task2.Id);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnOnlyUserTasks_WhenMultipleUsersExist()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        var user1Task = new TaskEntity(
            Guid.NewGuid(),
            "User 1 Task",
            "Description",
            TaskEntityStatus.Pending,
            TaskPriority.High,
            DateTime.UtcNow.AddDays(1),
            user1Id
        );

        var user2Task = new TaskEntity(
            Guid.NewGuid(),
            "User 2 Task",
            "Description",
            TaskEntityStatus.Pending,
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(2),
            user2Id
        );

        await _repository.CreateAsync(user1Task);
        await _repository.CreateAsync(user2Task);

        // Act
        var result = await _repository.GetAllByUserIdAsync(user1Id);

        // Assert
        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(user1Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTaskInDatabase_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskEntity(
            Guid.NewGuid(),
            "Original Title",
            "Original Description",
            TaskEntityStatus.Pending,
            TaskPriority.Low,
            DateTime.UtcNow.AddDays(5),
            userId
        );

        await _repository.CreateAsync(task);

        // Update the task
        task.UpdateDetails("Updated Title", "Updated Description", TaskPriority.Urgent, DateTime.UtcNow.AddDays(10));
        task.UpdateStatus(TaskEntityStatus.Completed);

        // Act
        var result = await _repository.UpdateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(TaskPriority.Urgent);
        result.Status.Should().Be(TaskEntityStatus.Completed);
        result.UserId.Should().Be(userId);

        // Verify it's actually updated in the database
        var retrievedTask = await _repository.GetByIdAsync(task.Id);
        retrievedTask.Should().NotBeNull();
        retrievedTask!.Title.Should().Be("Updated Title");
        retrievedTask.Description.Should().Be("Updated Description");
        retrievedTask.Priority.Should().Be(TaskPriority.Urgent);
        retrievedTask.Status.Should().Be(TaskEntityStatus.Completed);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task = new TaskEntity(
            Guid.NewGuid(),
            "Test Task",
            "Test Description",
            TaskEntityStatus.Pending,
            TaskPriority.Medium,
            DateTime.UtcNow.AddDays(3),
            userId
        );

        await _repository.CreateAsync(task);

        // Act
        var result = await _repository.GetByIdAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
        result.Title.Should().Be(task.Title);
        result.Description.Should().Be(task.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }
}
