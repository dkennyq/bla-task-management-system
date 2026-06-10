using TasksApi.Domain.Entities;

namespace TasksApi.Application.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
