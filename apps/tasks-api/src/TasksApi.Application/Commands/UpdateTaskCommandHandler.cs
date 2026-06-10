using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public class UpdateTaskCommandHandler : IUpdateTaskCommandHandler
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskEntity> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        // Get existing task
        var task = await _repository.GetByIdAsync(command.Id, cancellationToken);
        
        if (task == null)
        {
            throw new NotFoundException($"Task with ID {command.Id} not found");
        }

        // Check authorization - user can only update their own tasks
        if (task.UserId != command.UserId)
        {
            throw new ForbiddenException($"User {command.UserId} is not authorized to update task {command.Id}");
        }

        // Update task details
        task.UpdateDetails(command.Title, command.Description, command.Priority, command.DueDate);
        
        // Update status
        task.UpdateStatus(command.Status);

        // Persist changes
        var updatedTask = await _repository.UpdateAsync(task, cancellationToken);

        return updatedTask;
    }
}
