using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;

namespace TasksApi.Application.Commands;

public class DeleteTaskCommandHandler : IDeleteTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public DeleteTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        // Get existing task
        var task = await _repository.GetByIdAsync(command.Id, cancellationToken);
        
        if (task == null)
        {
            throw new NotFoundException($"Task with ID {command.Id} not found");
        }

        // Check authorization - user can only delete their own tasks
        if (task.UserId != command.UserId)
        {
            throw new ForbiddenException($"User {command.UserId} is not authorized to delete task {command.Id}");
        }

        // Delete the task
        await _repository.DeleteAsync(command.Id, cancellationToken);
    }
}
