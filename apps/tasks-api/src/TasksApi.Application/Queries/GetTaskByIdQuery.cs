namespace TasksApi.Application.Queries;

public record GetTaskByIdQuery(Guid Id, Guid UserId);
