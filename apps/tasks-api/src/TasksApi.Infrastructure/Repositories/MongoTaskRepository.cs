using MongoDB.Driver;
using TasksApi.Application.Interfaces;
using TasksApi.Domain.Entities;
using TasksApi.Infrastructure.Models;

namespace TasksApi.Infrastructure.Repositories;

public class MongoTaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskDocument> _collection;

    public MongoTaskRepository(string connectionString, string databaseName, string collectionName = "tasks")
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _collection = database.GetCollection<TaskDocument>(collectionName);

        // Crear índice por userId para queries más rápidas
        var indexKeys = Builders<TaskDocument>.IndexKeys.Ascending(t => t.UserId);
        var indexModel = new CreateIndexModel<TaskDocument>(indexKeys);
        _collection.Indexes.CreateOneAsync(indexModel).GetAwaiter().GetResult();
    }

    public async Task<IEnumerable<TaskEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _collection.Find(_ => true).ToListAsync(cancellationToken);
        return documents.Select(d => d.ToEntity());
    }

    public async Task<IEnumerable<TaskEntity>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TaskDocument>.Filter.Eq(t => t.UserId, userId);
        var documents = await _collection.Find(filter).ToListAsync(cancellationToken);
        return documents.Select(d => d.ToEntity());
    }

    public async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TaskDocument>.Filter.Eq(t => t.Id, id);
        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document?.ToEntity();
    }

    public async Task<TaskEntity> CreateAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        var document = TaskDocument.FromEntity(task);
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
        return task;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        var document = TaskDocument.FromEntity(task);
        var filter = Builders<TaskDocument>.Filter.Eq(t => t.Id, task.Id);
        await _collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
        return task;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TaskDocument>.Filter.Eq(t => t.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}
