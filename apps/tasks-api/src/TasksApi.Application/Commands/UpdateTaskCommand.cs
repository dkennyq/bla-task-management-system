using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public class UpdateTaskCommand
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskEntityStatus Status { get; set; } = TaskEntityStatus.Pending;
    public DateTime? DueDate { get; set; }
    public Guid UserId { get; set; }
}
