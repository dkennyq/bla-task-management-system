using TasksApi.Application.Exceptions;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;

namespace TasksApi.Application.Queries;

public class GetTaskByIdQueryHandler
{
    private readonly ITaskRepository _repository;

    public GetTaskByIdQueryHandler(ITaskRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<TaskEntity> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(query.Id));

        if (query.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(query.UserId));

        var task = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (task == null)
            throw new NotFoundException($"Task with id {query.Id} not found");

        if (task.UserId != query.UserId)
            throw new ForbiddenException("User does not have permission to access this task");

        return task;
    }
}
