using Npgsql;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, full_name, email, password_hash, created_at FROM users WHERE email = @Email";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new UserEntity(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("full_name")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.GetString(reader.GetOrdinal("password_hash"))
            );
        }

        return null;
    }

    public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, full_name, email, password_hash, created_at FROM users WHERE id = @Id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new UserEntity(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("full_name")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.GetString(reader.GetOrdinal("password_hash"))
            );
        }

        return null;
    }

    public async Task AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "INSERT INTO users (id, full_name, email, password_hash, created_at) VALUES (@Id, @FullName, @Email, @PasswordHash, @CreatedAt)";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
