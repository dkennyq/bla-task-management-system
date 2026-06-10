using Npgsql;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString;

    public RefreshTokenRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT id, user_id, token, expires_at, created_at, is_revoked FROM refresh_tokens WHERE token = @Token";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Token", token);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var entity = new RefreshTokenEntity(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("user_id")),
                reader.GetString(reader.GetOrdinal("token")),
                reader.GetDateTime(reader.GetOrdinal("expires_at"))
            );

            // Reflect DB state for revoked flag (private setter in domain)
            if (reader.GetBoolean(reader.GetOrdinal("is_revoked")))
                entity.Revoke();

            return entity;
        }

        return null;
    }

    public async Task AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "INSERT INTO refresh_tokens (id, user_id, token, expires_at, created_at, is_revoked) VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt, @IsRevoked)";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", refreshToken.Id);
        command.Parameters.AddWithValue("@UserId", refreshToken.UserId);
        command.Parameters.AddWithValue("@Token", refreshToken.Token);
        command.Parameters.AddWithValue("@ExpiresAt", refreshToken.ExpiresAt);
        command.Parameters.AddWithValue("@CreatedAt", refreshToken.CreatedAt);
        command.Parameters.AddWithValue("@IsRevoked", refreshToken.IsRevoked);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "UPDATE refresh_tokens SET is_revoked = TRUE WHERE user_id = @UserId AND is_revoked = FALSE";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@UserId", userId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
