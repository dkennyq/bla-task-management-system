namespace TasksApi.Application.Commands;

public interface IDeleteTaskCommandHandler
{
    Task Handle(DeleteTaskCommand command, CancellationToken cancellationToken);
}
