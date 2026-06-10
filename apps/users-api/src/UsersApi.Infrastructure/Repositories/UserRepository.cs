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

    private static UserEntity MapReader(NpgsqlDataReader reader)
    {
        return new UserEntity(
            reader.GetGuid(reader.GetOrdinal("id")),
            reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
            reader.GetString(reader.GetOrdinal("full_name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("password_hash"))
        );
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, username, full_name, email, password_hash, created_at FROM users WHERE email = @Email";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
            return MapReader(reader);

        return null;
    }

    public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, username, full_name, email, password_hash, created_at FROM users WHERE username = @Username";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Username", username);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
            return MapReader(reader);

        return null;
    }

    public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, username, full_name, email, password_hash, created_at FROM users WHERE id = @Id";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
            return MapReader(reader);

        return null;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT COUNT(1) FROM users WHERE email = @Email";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        var count = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
        return count > 0;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT COUNT(1) FROM users WHERE username = @Username";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Username", username);

        var count = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0);
        return count > 0;
    }

    public async Task AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "INSERT INTO users (id, username, full_name, email, password_hash, created_at) VALUES (@Id, @Username, @FullName, @Email, @PasswordHash, @CreatedAt)";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
