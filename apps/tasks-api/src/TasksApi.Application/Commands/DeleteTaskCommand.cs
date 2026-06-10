namespace TasksApi.Application.Commands;

public class DeleteTaskCommand
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
