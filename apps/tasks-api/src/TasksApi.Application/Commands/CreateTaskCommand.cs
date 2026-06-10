using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public class CreateTaskCommand
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid UserId { get; set; }
}
