using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;

namespace TasksApi.Application.Commands;

public class CreateTaskCommandHandler : ICreateTaskCommandHandler
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskEntity> Handle(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var taskEntity = new TaskEntity(
            id: Guid.NewGuid(),
            title: command.Title,
            description: command.Description,
            status: TaskEntityStatus.Pending,
            priority: command.Priority,
            dueDate: command.DueDate,
            userId: command.UserId
        );

        var createdTask = await _taskRepository.CreateAsync(taskEntity, cancellationToken);
        
        return createdTask;
    }
}
