using FluentAssertions;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Domain.Tests.Entities;

public class TaskTests
{
    [Fact]
    public void Task_ShouldHaveValidId_WhenCreated()
    {
        // Arrange & Act
        var task = new TaskEntity(
            id: Guid.NewGuid(),
            title: "Test Task",
            description: "Test Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Medium,
            dueDate: DateTime.UtcNow.AddDays(7),
            userId: Guid.NewGuid()
        );

        // Assert
        task.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Task_ShouldHaveRequiredProperties_WhenCreated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var title = "Test Task";
        var description = "Test Description";
        var dueDate = DateTime.UtcNow.AddDays(7);

        // Act
        var task = new TaskEntity(
            id: id,
            title: title,
            description: description,
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Medium,
            dueDate: dueDate,
            userId: userId
        );

        // Assert
        task.Id.Should().Be(id);
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.Status.Should().Be(TaskEntityStatus.Pending);
        task.Priority.Should().Be(TaskPriority.Medium);
        task.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
        task.UserId.Should().Be(userId);
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Task_ShouldThrowException_WhenTitleIsInvalid(string invalidTitle)
    {
        // Arrange & Act
        Action act = () => new TaskEntity(
            id: Guid.NewGuid(),
            title: invalidTitle,
            description: "Test Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Medium,
            dueDate: DateTime.UtcNow.AddDays(7),
            userId: Guid.NewGuid()
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*title*");
    }

    [Fact]
    public void Task_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange & Act
        Action act = () => new TaskEntity(
            id: Guid.NewGuid(),
            title: "Test Task",
            description: "Test Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Medium,
            dueDate: DateTime.UtcNow.AddDays(7),
            userId: Guid.Empty
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*userId*");
    }

    [Fact]
    public void Task_CanUpdateStatus()
    {
        // Arrange
        var task = new TaskEntity(
            id: Guid.NewGuid(),
            title: "Test Task",
            description: "Test Description",
            status: TaskEntityStatus.Pending,
            priority: TaskPriority.Medium,
            dueDate: DateTime.UtcNow.AddDays(7),
            userId: Guid.NewGuid()
        );

        // Act
        task.UpdateStatus(TaskEntityStatus.InProgress);

        // Assert
        task.Status.Should().Be(TaskEntityStatus.InProgress);
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
