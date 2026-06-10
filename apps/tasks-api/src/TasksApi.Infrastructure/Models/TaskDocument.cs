using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TasksApi.Domain.Entities;

namespace TasksApi.Infrastructure.Models;

public class TaskDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public string Status { get; set; } = string.Empty;

    [BsonElement("priority")]
    [BsonRepresentation(BsonType.String)]
    public string Priority { get; set; } = string.Empty;

    [BsonElement("dueDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DueDate { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    public static TaskDocument FromEntity(TaskEntity entity)
    {
        return new TaskDocument
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Status = entity.Status.ToString(),
            Priority = entity.Priority.ToString(),
            DueDate = entity.DueDate,
            UserId = entity.UserId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public TaskEntity ToEntity()
    {
        return new TaskEntity(
            Id,
            Title,
            Description,
            Enum.Parse<TaskEntityStatus>(Status),
            Enum.Parse<TaskPriority>(Priority),
            DueDate,
            UserId
        );
    }
}
