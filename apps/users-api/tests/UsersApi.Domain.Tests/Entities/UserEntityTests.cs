using FluentAssertions;
using UsersApi.Domain.Entities;

namespace UsersApi.Domain.Tests.Entities;

public class UserEntityTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValid()
    {
        var id = Guid.NewGuid();
        var entity = new UserEntity(id, "johndoe", "John Doe", "john@example.com", "hash123");

        entity.Id.Should().Be(id);
        entity.Username.Should().Be("johndoe");
        entity.FullName.Should().Be("John Doe");
        entity.Email.Should().Be("john@example.com");
        entity.PasswordHash.Should().Be("hash123");
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUsernameEmpty()
    {
        Action act = () => new UserEntity(Guid.NewGuid(), "", "John Doe", "john@example.com", "hash");
        act.Should().Throw<ArgumentException>().WithMessage("*Username cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUsernameTooShort()
    {
        Action act = () => new UserEntity(Guid.NewGuid(), "ab", "John Doe", "john@example.com", "hash");
        act.Should().Throw<ArgumentException>().WithMessage("*at least 3*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenFullNameEmpty()
    {
        Action act = () => new UserEntity(Guid.NewGuid(), "johndoe", "", "john@example.com", "hash");
        act.Should().Throw<ArgumentException>().WithMessage("*Full name cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailInvalid()
    {
        Action act = () => new UserEntity(Guid.NewGuid(), "johndoe", "John Doe", "notanemail", "hash");
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid email format*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenPasswordHashEmpty()
    {
        Action act = () => new UserEntity(Guid.NewGuid(), "johndoe", "John Doe", "john@example.com", "");
        act.Should().Throw<ArgumentException>().WithMessage("*Password hash cannot be empty*");
    }
}
