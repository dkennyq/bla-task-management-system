using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;

namespace TasksApi.Application.Queries;

public class GetAllTasksQueryHandler
{
    private readonly ITaskRepository _repository;

    public GetAllTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<TaskEntity>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(query.UserId));

        if (query.UserRole == "Manager")
            return await _repository.GetAllAsync(cancellationToken);

        return await _repository.GetAllByUserIdAsync(query.UserId, cancellationToken);
    }
}
