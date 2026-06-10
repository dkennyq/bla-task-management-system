using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public interface ICreateTaskCommandHandler
{
    Task<TaskEntity> Handle(CreateTaskCommand command, CancellationToken cancellationToken = default);
}
