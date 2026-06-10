using FluentAssertions;
using UsersApi.Domain.Entities;

namespace UsersApi.Domain.Tests.Entities;

public class RefreshTokenEntityTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgs()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var token = "some-refresh-token";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var entity = new RefreshTokenEntity(id, userId, token, expiresAt);

        entity.Id.Should().Be(id);
        entity.UserId.Should().Be(userId);
        entity.Token.Should().Be(token);
        entity.ExpiresAt.Should().Be(expiresAt);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        entity.IsRevoked.Should().BeFalse();
        entity.IsActive.Should().BeTrue();
        entity.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUserIdIsEmpty()
    {
        Action act = () => new RefreshTokenEntity(Guid.NewGuid(), Guid.Empty, "token", DateTime.UtcNow.AddDays(1));
        act.Should().Throw<ArgumentException>().WithMessage("*UserId*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenTokenIsInvalid(string? invalidToken)
    {
        Action act = () => new RefreshTokenEntity(Guid.NewGuid(), Guid.NewGuid(), invalidToken!, DateTime.UtcNow.AddDays(1));
        act.Should().Throw<ArgumentException>().WithMessage("*Token*");
    }

    [Fact]
    public void Revoke_ShouldSetIsRevokedToTrue()
    {
        var entity = new RefreshTokenEntity(Guid.NewGuid(), Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        entity.Revoke();
        entity.IsRevoked.Should().BeTrue();
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ShouldBeTrue_WhenExpiryPassed()
    {
        var entity = new RefreshTokenEntity(Guid.NewGuid(), Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(-1));
        entity.IsExpired.Should().BeTrue();
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_ShouldBeFalse_WhenRevoked()
    {
        var entity = new RefreshTokenEntity(Guid.NewGuid(), Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        entity.Revoke();
        entity.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_ShouldBeFalse_WhenExpired()
    {
        var entity = new RefreshTokenEntity(Guid.NewGuid(), Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(-1));
        entity.IsActive.Should().BeFalse();
    }
}
