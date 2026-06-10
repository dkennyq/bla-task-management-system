using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public interface IUpdateTaskCommandHandler
{
    Task<TaskEntity> Handle(UpdateTaskCommand command, CancellationToken cancellationToken);
}
